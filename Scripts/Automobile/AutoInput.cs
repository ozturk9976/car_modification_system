using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoInput : MonoBehaviour
{
    private PlayerInputActions m_Actions;


    private void OnEnable()
    {
        m_Actions.Enable();
    }

    private void OnDisable()
    {
        m_Actions.Disable();
    }

    void Awake()
    {
        m_Actions = new PlayerInputActions();
    }

    public Vector2 Get_MovementInputAxis
    {
        get { return m_Actions.Player.Move.ReadValue<Vector2>(); }
    }

    public float Get_BrakeInput
    {
        get { return m_Actions.Player.Brake.ReadValue<float>(); }
    }
}
