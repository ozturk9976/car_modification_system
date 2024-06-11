using System.Collections;
using UnityEngine;
using Cinemachine;
using DG.Tweening;
using UnityEngine.InputSystem;


public class CineExtension : MonoBehaviour
{


    public CinemachineInputProvider _cinemachineInputProvider;
    public int isTouchingButton;
    public CinemachineFreeLook vcam;
    // CinemachineOrbitalTransposer[] orbital = new CinemachineOrbitalTransposer[3];
    CinemachineVirtualCamera[] rigs = new CinemachineVirtualCamera[3];
    // private CinemachineTouchInputMapper InputMapper;
    private GameObject mainCam;
    private Transform carTransform;
    public bool isPanelOpened;
    // private bool canMove;
    public static CineExtension instance;

    public PlayerInput _playerInput;
    float originalFOV;

    private void Awake()
    {
        vcam.m_XAxis.Value = 0;
        vcam.m_YAxis.Value = 0.6f;
        instance = this;
        _playerInput = new PlayerInput();
        mainCam = GameObject.Find("Main Camera");
    }


    private void OnEnable()
    {
        /*
        _playerInput.Enable();

        EventManager.AddHandler(GameEvent.OnGameOver, StartTheCoroutine);
        EventManager.AddHandler(GameEvent.OnCameraCantMove, ResetAxisValues);
        EventManager.AddHandler(GameEvent.OnLevelPass, OnLevelPass);
        */
    }
    private void OnDisable()
    {/*
        _playerInput.Disable();

        EventManager.RemoveHandler(GameEvent.OnGameOver, StartTheCoroutine);
        EventManager.RemoveHandler(GameEvent.OnCameraCantMove, ResetAxisValues);
        EventManager.RemoveHandler(GameEvent.OnLevelPass, OnLevelPass);*/
    }


    private void Start()
    {
        originalFOV = vcam.m_Lens.FieldOfView;
        if (!_cinemachineInputProvider.enabled)
        {
            _cinemachineInputProvider.enabled = true;
        }

        DOTween.To(() => vcam.m_XAxis.Value, x => vcam.m_XAxis.Value = x, 0f, 0.2f);
        DOTween.To(() => vcam.m_YAxis.Value, x => vcam.m_YAxis.Value = x, 0.6f, 0.2f);

    }



    void LateUpdate()
    {
        if (Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.OSXEditor)
        {
            if (CineExtension.instance.isTouchingButton == 0 && !CineFreeZoom.instance.isZooming)
            {
                if (!_cinemachineInputProvider.enabled)
                {
                    _cinemachineInputProvider.enabled = true;
                }

            }
            else
            {
                if (_cinemachineInputProvider.enabled)
                {
                    _cinemachineInputProvider.enabled = false;
                }
            }
        }

    }

    public void InitalizeCameraPosition(Vector3 targetPos)
    {

    }


    public void ReCenter()
    {
        DOTween.To(() => vcam.m_XAxis.Value, x => vcam.m_XAxis.Value = x, 0, 0.7f).SetEase(Ease.OutQuart);
        DOTween.To(() => vcam.m_YAxis.Value, y => vcam.m_YAxis.Value = y, 0.6f, 0.7f).SetEase(Ease.OutQuart);
    }

    public void ResetLookatTransform()
    {
        carTransform = GameObject.FindGameObjectWithTag("Player").transform;
        vcam.m_LookAt = carTransform;
    }
    public void GameOverCamera(Transform collidedObject, Transform newFollowObj)
    {
        SwitchToWorldSpace();
        // ResetAxisValues();
        // StartCoroutine(_ProcessShake(1));
        // vcam.m_CommonLens = enabled;
        // vcam.m_Lens.FieldOfView = 60;

        for (int i = 0; i < 3; i++)
        {
            vcam.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XDamping = 5;
            vcam.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_YDamping = 5;
            vcam.GetRig(i).GetCinemachineComponent<CinemachineOrbitalTransposer>().m_ZDamping = 5;
            vcam.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_VerticalDamping = 5;
            vcam.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_HorizontalDamping = 5;

        }


        //vcam.m_LookAt = collidedObject;
        vcam.m_LookAt = collidedObject;
        vcam.m_Follow = collidedObject;
        // vcam.m_XAxis.m_AccelTime = 2f;
        // vcam.m_YAxis.m_AccelTime = 2f;
        // SwitchToWorldSpace();    
    }

    public void SwitchToWorldSpace()
    {
        vcam.m_BindingMode = CinemachineTransposer.BindingMode.SimpleFollowWithWorldUp;
        //vcam.InternalUpdateCameraState(Vector3.up, -1);
        //vcam.m_XAxis.Value = 0;
        //vcam.m_YAxis.Value = 0;
        //vcam.PreviousStateIsValid = false;
    }

    public void DontMove()
    {
        _cinemachineInputProvider.enabled = false;
    }
    public void Move()
    {
        _cinemachineInputProvider.enabled = true;
    }


    private void StartTheCoroutine()
    {
        OnGameOver();
        StartCoroutine(LerpCamera());
    }

    public void ChangeFov(float _desiredFov)
    {
        DOTween.To(() => vcam.m_Lens.FieldOfView, x => vcam.m_Lens.FieldOfView = x, _desiredFov, 1f).SetEase(Ease.OutSine);
    }

    public void ResetFov()
    {
        DOTween.To(() => vcam.m_Lens.FieldOfView, x => vcam.m_Lens.FieldOfView = x, originalFOV, 1f).SetEase(Ease.OutSine);
    }

    public void ChangeTrackedObjectOffset(Vector3 targetOffset)
    {
        // for (int i = 0; i < 3; i++)
        // {
        //     vcam.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = targetOffset;
        // }
    }

    public void ResetTrackedObjectOffset()
    {
        // for (int i = 0; i < 3; i++)
        // {
        //     vcam.GetRig(i).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset = Vector3.zero;
        // }
    }


    IEnumerator LerpCamera()
    {
        while (true)
        {
            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, 45, 0.1f * Time.unscaledDeltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public void ResetAxisValues()
    {
        vcam.m_XAxis.m_InputAxisValue = 0;
        vcam.m_YAxis.m_InputAxisValue = 0;
    }

    public void Shake(float amplitudeGain, float frequencyGain, float time)
    {
        StartCoroutine(ShakeIE(amplitudeGain, frequencyGain, time));
    }

    private IEnumerator ShakeIE(float amplitudeGain, float frequencyGain, float time)
    {

        CinemachineVirtualCamera[] rigs = { vcam.GetRig(0), vcam.GetRig(1), vcam.GetRig(2) };

        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitudeGain;
            rigs[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequencyGain;
        }

        yield return new WaitForSecondsRealtime(time);

        for (int i = 0; i < rigs.Length; i++)
        {
            rigs[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
            rigs[i].GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;
        }

    }

    private void OnLevelPass()
    {
        ResetAxisValues();
        isTouchingButton = 0;
    }
    private void OnGameOver()
    {
        ResetAxisValues();
        isTouchingButton = 0;
    }
}
