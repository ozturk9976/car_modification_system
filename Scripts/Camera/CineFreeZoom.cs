using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;

public class CineFreeZoom : MonoBehaviour
{

    // [SerializeField] private float referenceOrbitRadius;
    public bool isZooming;
    public bool canZoom;
    public static CineFreeZoom instance;
    [SerializeField] private float orbitRadius;
    private CinemachineFreeLook.Orbit[] originalOrbits;
    private CinemachineFreeLook vcam;
    float diff;
    float lastOrbitRadius;
    //PlayerInput _playerInput;

    private void Awake()
    {
        instance = this;
        EnhancedTouchSupport.Enable();
        //_playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        //_playerInput.Enable();
        vcam = GetComponent<CinemachineFreeLook>();

        // // lastOrbitRadius = PlayerPrefs.GetFloat("CineOrbitsRadius", 9);
        // for (int i = 0; i < vcam.m_Orbits.Length; i++)
        // {
        //     vcam.m_Orbits[i].m_Radius = lastOrbitRadius;
        // }

        //EventManager.AddHandler(GameEvent.OnCameraZoom, OnCanZoom);
        //EventManager.AddHandler(GameEvent.OffCameraZoom, OnCantZoom);
    }
    private void OnDisable()
    {
        //_playerInput.Disable();
        // PlayerPrefs.SetFloat("CineOrbitsRadius", vcam.m_Orbits[0].m_Radius);
        //EventManager.RemoveHandler(GameEvent.OnCameraZoom, OnCanZoom);
        //EventManager.AddHandler(GameEvent.OffCameraZoom, OnCantZoom);
    }
    void Start()
    {
        originalOrbits = new CinemachineFreeLook.Orbit[vcam.m_Orbits.Length];
        for (int i = 0; i < originalOrbits.Length; i++)
        {
            originalOrbits[i].m_Radius = vcam.m_Orbits[i].m_Radius;
            originalOrbits[i].m_Height = vcam.m_Orbits[i].m_Height;
        }
    }
    public void OnCantZoom()
    {
        canZoom = false;
    }
    public void OnCanZoom()
    {
        canZoom = true;
    }


    void LateUpdate()
    {
        #region Old input system
        // if (Application.platform == RuntimePlatform.WindowsEditor)
        // {
        //     // float difference = Input.mouseScrollDelta.y * 90;
        //     float difference = _playerInput.Gameplay.CinemachineCameraZoom.ReadValue<Vector2>().y;
        //     float diff = difference * 0.1f * Time.deltaTime;

        //     for (int i = 0; i < vcam.m_Orbits.Length; i++)
        //     {
        //         vcam.m_Orbits[i].m_Radius += originalOrbits[i].m_Radius * -diff;
        //         vcam.m_Orbits[i].m_Radius = Mathf.Clamp(vcam.m_Orbits[i].m_Radius, originalOrbits[i].m_Radius - 2.8f, originalOrbits[i].m_Radius + 3);
        //     }
        // }

        // if (InputManager.isTouchingButton == 0 && Input.touchCount == 2)
        // {
        //     if (!canZoom)
        //     {
        //         canZoom = true;
        //     }
        // }
        // else
        // {
        //     if (canZoom)
        //     {
        //         canZoom = false;
        //     }
        // }

        // if (canZoom)
        // {
        //     isZooming = true;
        //     Touch touch1 = Input.GetTouch(0);
        //     Touch touch2 = Input.GetTouch(1);

        //     Vector2 touchFirstPrev = touch1.position - touch1.deltaPosition;
        //     Vector2 touchSecondPrev = touch2.position - touch2.deltaPosition;

        //     float prevMagnitude = (touchFirstPrev - touchSecondPrev).magnitude;
        //     float currentMagnitude = (touch1.position - touch2.position).magnitude;
        //     float difference = currentMagnitude - prevMagnitude;

        //     float diff = difference /*_playerInput.Gameplay.CinemachineCameraZoom.ReadValue<Vector2>().y*/ * 0.1f * Time.deltaTime;

        //     for (int i = 0; i < vcam.m_Orbits.Length; i++)
        //     {
        //         vcam.m_Orbits[i].m_Radius += originalOrbits[i].m_Radius * -diff;
        //         vcam.m_Orbits[i].m_Radius = Mathf.Clamp(vcam.m_Orbits[i].m_Radius, originalOrbits[i].m_Radius - 3f, originalOrbits[i].m_Radius + 3f);
        //     }
        // }
        // else
        // {
        //     isZooming = false;
        // }

        #endregion

        /// NEW INPUT
        // float diff = _playerInput.Gameplay.CinemachineCameraZoom.ReadValue<Vector2>().y;
        var _touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
        if (_touches.Count == 2 && CineExtension.instance.isTouchingButton == 0)
        {

            isZooming = true;

            var touch1 = _touches[0];
            var touch2 = _touches[1];

            Vector2 touchFirstPrev = touch1.finger.screenPosition - touch1.delta;
            Vector2 touchSecondPrev = touch2.finger.screenPosition - touch2.delta;

            float prevMagnitude = (touchFirstPrev - touchSecondPrev).magnitude;
            float currentMagnitude = (touch1.finger.screenPosition - touch2.finger.screenPosition).magnitude;
            float difference = currentMagnitude - prevMagnitude;
            float diff = difference * 0.1f * Time.deltaTime;

            for (int i = 0; i < originalOrbits.Length; i++)
            {
                vcam.m_Orbits[i].m_Radius += originalOrbits[i].m_Radius * -diff;
                vcam.m_Orbits[i].m_Radius = Mathf.Clamp(vcam.m_Orbits[i].m_Radius
               , originalOrbits[i].m_Radius - 1f, originalOrbits[i].m_Radius + 1.4f);



                /*
                    vcam.m_Orbits[i].m_Height += originalOrbits[i].m_Height * -diff;
                    vcam.m_Orbits[i].m_Height = Mathf.Clamp(vcam.m_Orbits[i].m_Height, originalOrbits[i].m_Height - 1.2f, originalOrbits[i].m_Height + 1f);
                */

                if (i != 2)
                {
                    vcam.m_Orbits[i].m_Height += originalOrbits[i].m_Height * -diff;
                    vcam.m_Orbits[i].m_Height = Mathf.Clamp(vcam.m_Orbits[i].m_Height, originalOrbits[i].m_Height - 1.2f, originalOrbits[i].m_Height + 1.2f);
                }

            }
        }
        else
        {
            isZooming = false;
        }

        //DEBUG REASONS
#if UNITY_EDITOR
        /*
                Debug.Log("Unity Editor");

                if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
                {
                    // float difference = Input.mouseScrollDelta.y * 90;
                    float difference = _playerInput.Gameplay.CinemachineCameraZoom.ReadValue<Vector2>().y;
                    float diff = difference * 0.1f * Time.deltaTime;

                    for (int i = 0; i < originalOrbits.Length; i++)
                    {
                        vcam.m_Orbits[i].m_Radius += originalOrbits[i].m_Radius * -diff;
                        vcam.m_Orbits[i].m_Radius = Mathf.Clamp(vcam.m_Orbits[i].m_Radius, originalOrbits[i].m_Radius - 1f, originalOrbits[i].m_Radius + 2f);

                        if (i != 2)
                        {
                            vcam.m_Orbits[i].m_Height += originalOrbits[i].m_Height * -diff;
                            vcam.m_Orbits[i].m_Height = Mathf.Clamp(vcam.m_Orbits[i].m_Height, originalOrbits[i].m_Height - 1.2f, originalOrbits[i].m_Height + 1.2f);
                        }

                    }

                }
        */
#endif
    }

    private void OnDestroy()
    {
        EnhancedTouchSupport.Disable();
    }
}

