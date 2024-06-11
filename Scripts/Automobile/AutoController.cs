using NWH.Common.Vehicles;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using NWH.WheelController3D;
using System.Net.Sockets;
using Unity.VisualScripting;


[Serializable]
public class _Automobile
{
    public float motorPower = 50;
    public float maxBrakeTorque = 4000;
    public float maxSteeringAngle = 40;
    public float minSteeringAngle = 25;

}

[Serializable]
public class _Wheel
{
    public bool motor;
    public bool steering;
    public bool brake;
    public WheelUAPI wheelUAPI;
}

public enum GearState
{
    Neutral,
    Running,
    CheckingChange,
    Changing
};



[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AutoEngineAudio))]
[RequireComponent(typeof(AutoInput))]
[RequireComponent(typeof(AutoParts))]


public class AutoController : Auto
{
    [SerializeField] AutoInput automobileInput;
    [SerializeField] AutoParts automobileParts;

    [Header("Car Settings")]
    [SerializeField] private AnimationCurve enginePower;
    [SerializeField] private _Automobile automobile;
    public List<_Wheel> wheels;

    [SerializeField] float changeGearTime = 0.5f;
    [SerializeField] float[] gearRatios;
    [SerializeField] float differantialRatio = 2;
    [SerializeField] float redLine = 4500;
    [SerializeField] float increaseGearRPM = 3250;
    [SerializeField] float decreaseGearRPM = 1700;
    [SerializeField] float idleEngineRPM = 400;

    //Privates
    private float isEngineRunning = 0;
    private float smoothXAxis;
    private float xAxisVelocity;
    private float engineRPM;
    private float xInputAxis;
    private float yInputAxis;
    private float currentTorque;
    private float clutch;
    private GearState gearState;
    private float[] wheelsGrounedRPMs;
    private float shiii;
    private float climbFactor;

    //Input
    private Vector2 movementInputAxis;
    private bool brakeHeld;
    private bool gasHeld;
    private int currentGearNum;

    public static AutoController instance;

    public delegate void OnChassisChanged(Transform FR, Transform FL, Transform RR, Transform RL);
    public OnChassisChanged onChassisChanged;


    public override void Awake()
    {
        base.Awake();

        instance = this;

        vehicleTransform = transform;
        vehicleRigidbody = GetComponent<Rigidbody>();
        vehicleRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    private void OnEnable()
    {
        CustomizationManager.onChassisChanged += SetNewWheelVisuals;
    }

    private void OnDisable()
    {
        CustomizationManager.onChassisChanged -= SetNewWheelVisuals;
    }

    void Start()
    {
        wheelsGrounedRPMs = new float[4];
        isEngineRunning = 1;
        //Application.targetFrameRate = 60;
    }

    void Update()
    {
        movementInputAxis = automobileInput.Get_MovementInputAxis;
        brakeHeld = automobileInput.Get_BrakeInput > 0.1f;
        gasHeld = yInputAxis > 0;
    }

    void LateUpdate()
    {
        if (Mathf.Abs(yInputAxis) > 0 && isEngineRunning == 0)
        {
            //StartCoroutine(GetComponent<AutoEngineAudio>().StartEngine());
            gearState = GearState.Running;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (gearState != GearState.Changing)
        {
            if (gearState == GearState.Neutral)
            {
                clutch = 0;
                if (yInputAxis > 0)
                {
                    gearState = GearState.Running;
                }
            }
            else
            {
                clutch = Input.GetKey(KeyCode.LeftShift) ? 0 : Mathf.Lerp(clutch, 1, Time.deltaTime);
            }
        }

        ApplyClimbFactor(); 
        RegulateInput();
        ApplySystems();
    }

    private void RegulateInput()
    {
        xInputAxis = Mathf.Round(movementInputAxis.x);
        yInputAxis = Mathf.Round(movementInputAxis.y);
        smoothXAxis = Mathf.SmoothDamp(smoothXAxis, xInputAxis, ref xAxisVelocity, 0.12f);
        shiii = gasHeld ? Mathf.Lerp(shiii, 1.2f, Time.deltaTime * 0.9f) : Mathf.Lerp(shiii, 0, Time.deltaTime * 1f);
    }

    private void ApplyClimbFactor()
    {
        float currentAngle = Mathf.Abs(Vector3.Angle(-Vector3.up, transform.forward));
        climbFactor = Mathf.Lerp(climbFactor, currentAngle / 35, Time.deltaTime); // 45 dereceye kadar lineer artış
    }

    private void ApplySystems()
    {
        for (int i = 0; i < wheels.Count; i++)
        {
            _Wheel wheel = wheels[i];
            WheelUAPI wc = wheels[i].wheelUAPI;
            wc.BrakeTorque = 0f;
            wc.MotorTorque = 0f;

            if (wheel.motor)
            {
                currentTorque = CalculateTorque() * climbFactor * shiii;
                wc.MotorTorque = currentTorque * yInputAxis;
            }

            if (wheel.brake && brakeHeld)
            {
                wc.BrakeTorque = automobile.maxBrakeTorque;
            }

            if (wheel.steering)
            {
                wc.SteerAngle = Mathf.Lerp(automobile.maxSteeringAngle, automobile.minSteeringAngle, Speed * 0.04f) * smoothXAxis;
            }

            if (wc.IsGrounded)
            {
                wheelsGrounedRPMs[i] = wc.RPM;
            }
        }
    }

    private float CalculateTorque()
    {
        float torque = 0;

        if (engineRPM < idleEngineRPM + 200 && yInputAxis == 0 && currentGearNum == 0)
        {
            gearState = GearState.Neutral;
        }

        if (gearState == GearState.Running && clutch > 0)
        {
            if (engineRPM > increaseGearRPM)
            {
                StartCoroutine(ChangeGear(1));
            }
            else if (engineRPM < decreaseGearRPM)
            {
                StartCoroutine(ChangeGear(-1));
            }
        }

        if (isEngineRunning > 0)
        {
            if (clutch < 0.1f)
            {
                engineRPM = Mathf.Lerp(engineRPM, Mathf.Max(idleEngineRPM, redLine * yInputAxis) + UnityEngine.Random.Range(-50, 50), Time.deltaTime);
            }
            else
            {
                float wheelRPM = WheelRPM() * gearRatios[currentGearNum] * differantialRatio;
                engineRPM = Mathf.Lerp(engineRPM, Mathf.Max(idleEngineRPM - 100, wheelRPM), Time.deltaTime * 3f);
                torque = (enginePower.Evaluate(engineRPM / redLine) * (automobile.motorPower / engineRPM)) * gearRatios[currentGearNum] * differantialRatio * 5252 * clutch;
            }
        }
        return torque;

    }

    public float GetSpeedRatio()
    {
        var gas = Mathf.Clamp(Mathf.Abs(yInputAxis), 0.5f, 1f);
        return engineRPM * gas / redLine;
    }

    //Linq kullanma update te garbage üretir
    private float WheelRPM()
    {
        float wheelsRPM = wheelsGrounedRPMs.Average();
        return wheelsRPM;
    }

    IEnumerator ChangeGear(int gearChange)
    {
        gearState = GearState.CheckingChange;

        if (currentGearNum + gearChange >= 0)
        {
            if (gearChange > 0)
            {
                yield return new WaitForSeconds(0.2f); //lastikler spin atıyormu vs diye emin olmak için tekrar check edilir
                if (engineRPM < increaseGearRPM || currentGearNum >= gearRatios.Length - 1)
                {
                    gearState = GearState.Running;
                    yield break;
                }
            }
            if (gearChange < 0)
            {
                yield return new WaitForSeconds(0.2f);
                if (engineRPM > decreaseGearRPM || currentGearNum <= 0)
                {
                    gearState = GearState.Running;
                    yield break;
                }

            }
            gearState = GearState.Changing;
            yield return new WaitForSeconds(changeGearTime);
            currentGearNum += gearChange;
        }

        if (gearState != GearState.Neutral)
            gearState = GearState.Running;
    }

    public void SetNewWheelVisuals()
    {
        //Debug.Log("Set New Wheel Visuals worked");
        for (int i = 0; i < wheels.Count; i++)
        {
            wheels[i].wheelUAPI.NonRotatingVisual = AutoParts.GetWheelNonVisuals()[i];
        }
    }

    private void Reset()
    {
        redLine = 4500;
        increaseGearRPM = 3250;
        decreaseGearRPM = 1700;
        idleEngineRPM = 400;
        changeGearTime = 0.5f;
        differantialRatio = 2;

        automobileInput = GetComponent<AutoInput>();
        automobileParts = GetComponent<AutoParts>();

        gearRatios = new float[] { 3, 1.5f };


        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.mass = 1500;
        rigidbody.angularDrag = 0.1f;
        rigidbody.drag = 0.1f;
        rigidbody.automaticCenterOfMass = false;
        rigidbody.centerOfMass = new Vector3(0, -0.15f, 0);
        rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        

        enginePower = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

        wheels = new List<_Wheel>();
        WheelUAPI[] wheelUAPIs = GetComponentsInChildren<WheelUAPI>();
        for (int i = 0; i < wheelUAPIs.Length; i++)
        {
            WheelUAPI wheelUAPI = wheelUAPIs[i];
            wheels.Add(new _Wheel()
            {
                wheelUAPI = wheelUAPI,
                steering = i < 2,
                motor = true,
                brake = i > 1
            });
        }


        
        

        /*
        List<GameObject> wheelnonVisuals = AutoParts.GetWheelNonVisuals();
        string[] wheelNames = {"WC_FL","WC_FR","WC_RR","WC_RL"};
        for (int i = 0; i < wheelnonVisuals.Count; i++)
        {
            GameObject newWheel = new GameObject();
            newWheel.AddComponent<WheelController>();
            newWheel.name = wheelNames[i];
            newWheel.transform.SetParent(transform.parent);
        }
        */
    }
}
