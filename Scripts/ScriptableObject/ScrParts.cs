using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using ENUMS;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using MoreMountains.Tools;
using UnityEditor;
using static UnityEditor.Progress;


[System.Serializable]
public class PartData
{
    public int partPrice;
    public GameObject partGameObject;
}


[CreateAssetMenu(menuName = "Scriptable Objects / Parts")]
public class ScrParts : ScriptableObject
{
    [Tooltip("The action to perform")]
    public event Action EventName;

    public string socketName;

    public PartType partType;

    public GameObject[] partPrefabs;

    public List<PartData> PartDatas;

    private int currentPart = 0;

    private string currenPartName;

    public GameObject GetNextPart()
    {
        currentPart++;
        if (currentPart >= PartDatas.Count)
        {
            currentPart = 0;
        }
       //Debug.Log(currentPart);
        return PartDatas[currentPart].partGameObject;
    }

    public GameObject GetPrevPart()
    {
        if (currentPart - 1 < 0)
        {
            currentPart = PartDatas.Count - 1;
        }
        else
        {
            currentPart--;
        }
        return PartDatas[currentPart].partGameObject;
        
    }

    /// <summary>
    /// Maybe make it virtual for grill or hole bonnets system
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual GameObject GetGameobjectByName(string name) 
    {
        if (name.Contains("Clone")) 
        {
            name = name.Substring(0, name.Length - 7);
        }


        GameObject _gameobject = null;

        foreach (var item in PartDatas)
        {
            if (item.partGameObject.name == name)
            {
                _gameobject = item.partGameObject;                
                return _gameobject;
            }
        }
        Debug.LogError("Obj is Null" + "tried to get by string -> " + name);
        return null;
    }

    /*
    public void BuyPart(string carName,GameObject part) 
    {
        string saveName = null;
        for (int i = 0;i < parts.Length;i++) 
        {
            if (parts[i].name == part.name) 
            {
                saveName = parts[i].name;   
            }
        }

        PlayerPrefs.SetString(name, saveName);
        Debug.Log(part.ToString());
    }
    */

    public GameObject LoadPart() 
    {
        return null;
    }


    [MMInspectorButton("OnButtonClicked")]
    public bool clickMe;

    public void OnButtonClicked()
    {
        ScrParts scrParts = this;
        for (global::System.Int32 i = 0; i < scrParts.partPrefabs.Length; i++)
        {
            PartData _partData = new()
            {
                partGameObject = scrParts.partPrefabs[i],
                partPrice = 1200
            };
            scrParts.PartDatas.Add(_partData);
        }
    }
}
/*

[CustomEditor(typeof(ScrParts))]
public class ScrPartsOv : Editor
{
    public override void OnInspectorGUI()
    {
        //MyPlayer targetPlayer = (MyPlayer)target;
        EditorGUILayout.LabelField("Some help", "Some other text");
        ScrParts scrParts = (ScrParts)target; // ScrParts örneðini al

        if (GUILayout.Button("Set All Parts!"))
        {
            for (global::System.Int32 i = 0; i < scrParts.partPrefabs.Length; i++)
            {
                PartData _partData = new()
                {
                    partGameObject = scrParts.partPrefabs[i],
                    partPrice = 1200
                };
                scrParts.PartDatas.Add(_partData);
            }
        };

        // Show default inspector property editor
        if (DrawDefaultInspector())
            Debug.Log("Gear was changed!"); 
    }
}
*/
