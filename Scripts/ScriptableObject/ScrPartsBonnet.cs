using MoreMountains.Feel;
using MoreMountains.Tools;
using ENUMS;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;



[CreateAssetMenu(menuName = "Scriptable Objects / Parts_Bonnet")]
public class ScrPartsBonnet : ScrParts
{
    public ENUMS.BonnetTypes Get_BonnetType() 
    {
        Debug.Log("working");
        for (int i = 0; i < PartDatas.Count; i++)
        {
            PartData item = PartDatas[i];
            string name = item.partGameObject.name;

            if (name.Contains("Grill"))
            {
                return ENUMS.BonnetTypes.Grill;
            }
            else if (name.Contains("Hole"))
            {
                return ENUMS.BonnetTypes.Hole;
            }
            else 
            {
                return ENUMS.BonnetTypes.Default;
            }
        }

        Debug.LogError("Error! -> DEFAULT BONNET TYPE RETURNED");
        return ENUMS.BonnetTypes.Default;
    }
}
