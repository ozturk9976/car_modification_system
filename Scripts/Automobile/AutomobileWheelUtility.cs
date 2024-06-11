using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AutomobileWheelUtility : MonoBehaviour
{
    public float wheelRadius;
    public float wheelWidht;
    public GameObject wheelCenter;


    private void OnDrawGizmos()
    {
        Gizmos.color = new Color32(255,255,255,25);
        Gizmos.DrawCube(wheelCenter.transform.position, new Vector3(wheelWidht, wheelRadius, wheelRadius));      
    }

    private void Reset()
    {
        foreach (Transform item in transform)
        {
            if (item.name.Contains("Center"))
            {
                wheelCenter = item.gameObject;
            }
        }
    }

}
