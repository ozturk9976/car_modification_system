using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Auto : MonoBehaviour
{
    public Vector3 LocalVelocity { get; private set; }
    public float LocalForwardVelocity { get; private set; }

    [NonSerialized]
    public Rigidbody vehicleRigidbody;

    [NonSerialized]
    public Transform vehicleTransform;

    public Vector3 Velocity { get; protected set; }

    public virtual void Awake()
    {
        vehicleTransform = transform;
        vehicleRigidbody = GetComponent<Rigidbody>();
        vehicleRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
    }

    public virtual void FixedUpdate()
    {
        Velocity = vehicleRigidbody.velocity;
        LocalVelocity = transform.InverseTransformDirection(Velocity);
        LocalForwardVelocity = LocalVelocity.z;
    }

    public float Speed
    {
        get { return LocalForwardVelocity < 0 ? -LocalForwardVelocity : LocalForwardVelocity; }
    }

}
