using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class UICustPartButtons : MonoBehaviour
{
    [SerializeField] private ScrParts scrParts;

    /// <summary>
    /// Invoked from UI button
    /// Setting new types of parts
    /// </summary>
    public void Clicked()
    {
        CustomizationManager.onTypeChanged.Invoke(scrParts);
    }
}
