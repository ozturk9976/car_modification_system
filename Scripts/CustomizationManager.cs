using ENUMS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class CustomizationManager : MonoBehaviour
{
    [SerializeField] private UIPartCountPanel uIPartCountPanel;
    [SerializeField] private ScrParts defaultScriptableObject;


    #region EVENTS

    public delegate void OnPartChange(ScrParts scrParts, bool isNext, PartType partType);
    public static OnPartChange onPartChanged;

    public delegate void OnBonnetChange();
    public static OnBonnetChange onBonnetChanged;

    public delegate void OnChassisChange();
    public static OnChassisChange onChassisChanged;

    public delegate void OnPartSave();
    public static OnPartSave onPartSaved;

    public delegate void OnTypeChange(ScrParts scrParts);
    public static OnTypeChange onTypeChanged;

    public delegate void OnTireChange(float radius, float widht, GameObject nonRotateVisual, GameObject visual);
    public static OnTireChange onTireChanged;

    #endregion

    public static ScrParts currentScrParts;

    void OnEnable()
    {
        onTypeChanged += SetType;
        onTypeChanged += SetCountPanel;     
    }

    void OnDisable()
    {
        onTypeChanged -= SetType;
        onTypeChanged -= SetCountPanel;
    }

    private void Start()
    {
        if (currentScrParts == null) currentScrParts = defaultScriptableObject;
    }

    void SetType(ScrParts scrParts)
    {
        currentScrParts = scrParts;
    }

    void SetCountPanelObjColor() 
    {

    }

    void SetCountPanel(ScrParts scrParts) 
    {
        uIPartCountPanel.CreateCountObjs(scrParts.PartDatas.Count);
    }

    public void ChangePartButtonClicked(bool isNext)
    {
        onPartChanged.Invoke(currentScrParts, isNext,currentScrParts.partType);
        //uIPartCountPanel.setcolorandshie
    }
}
