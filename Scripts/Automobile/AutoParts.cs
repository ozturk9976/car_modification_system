using System.Collections.Generic;
using UnityEngine;
using ENUMS;
using System;
using BayatGames.SaveGameFree;
using Unity.VisualScripting;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;




[System.Serializable]
public class Part
{
    [NonSerialized] public Transform socket;
    [SerializeField] public GameObject currentPart;
    public PartType partType;
    public string currentPartName;
    public string socketName;
}

public class AutoParts : MonoBehaviour
{
    [SerializeField] AutoController autoController;
    [SerializeField] List<Part> parts;
    public Transform Sockets;

    [SerializeField] private ScrDefaultParts defaultParts;
    //[SerializeField] private ScrParts bonnet;
    [SerializeField] private ScrPartsBonnet bonnet;
    [SerializeField] private ScrParts frontChassis;
    [SerializeField] private ScrParts rearChassis;
    [SerializeField] private ScrParts bumperFront;
    [SerializeField] private ScrParts bumperRear;
    [SerializeField] private ScrParts tires;
    [SerializeField] private ScrParts motor;
    [SerializeField] private ScrParts motorCarb;
    [SerializeField] private ScrParts motorExhaustLeft;
    [SerializeField] private ScrParts motorExhaustRight;


    public List<GameObject> currentWheelPrefabs = new();
    public List<Transform> sockets = new();
    public static List<GameObject> wheelNonVisuals = new();
    private Dictionary<PartType, ScrParts> scrPartsToType = new();
    private Dictionary<ScrParts, PartType> partTypeToScrParts = new();
    private Dictionary<ScrParts, string> scrPartsToCacheName = new();


    public string _cachedFrontChassisName;
    public string _cachedFrontBumperName;
    public string _cachedMotorName;
    public string _cachedMotorCarbName;
    public string _cachedMotorExhaustLeftName;
    public string _cachedMotorExhaustRightName;


    readonly string[] socketNames =
    {
        "SOCKET_Attachment_Door_Right",
        "SOCKET_Bonnet",
        "SOCKET_Bumper_Rear",
        "SOCKET_Char",
        "SOCKET_Chassis_Rear",
        "SOCKET_Fenders_Rear",
        "SOCKET_Fenders_Step",
        "SOCKET_Bumper_Front",
        "SOCKET_Chassis_Front",
        "SOCKET_Fenders_Front",
        "SOCKET_Motor",
        "SOCKET_Motor_Exhaust",
        "SOCKET_Motor_Carb",
        "SOCKET_Wheel_Spare",
        "SOCKET_Wheel_Front_Left",
        "SOCKET_Wheel_Front_Right",
        "SOCKET_Wheel_Rear_Left",
        "SOCKET_Wheel_Rear_Right"
    };

    readonly string[] tireSocketNames =
    {
        "SOCKET_Wheel_Front_Right",
        "SOCKET_Wheel_Front_Left",
        "SOCKET_Wheel_Rear_Right",
        "SOCKET_Wheel_Rear_Left"
    };

    /// <summary>
    /// belki de her bir socketin kar��l��� olan obje t�r� bir dictionary ye eklenebilir b�ylece rastgele d�eilde s�rayla socket olu�turulur sonra obje olu�turulur
    /// yoksa �nce t�m soketler sonra da t�m objeler olu�tuturlmaz
    /// </summary>

    private void Awake()
    {
        scrPartsToType = new Dictionary<PartType, ScrParts>
        {
            { PartType.Bonnet,bonnet},
            { PartType.FrontBumper,bumperFront },
            { PartType.FrontChassis,frontChassis },
            { PartType.RearBumper,bumperRear },
            { PartType.RearChassis,rearChassis },
            { PartType.Motor,motor },
            { PartType.MotorCarb,motorCarb },
            { PartType.MotorExhaustRight,motorExhaustRight },
            { PartType.MotorExhaustLeft,motorExhaustLeft }
        };

        partTypeToScrParts = new Dictionary<ScrParts, PartType>
        {
            { bonnet , PartType.Bonnet},
            { bumperFront , PartType.FrontBumper},
            { frontChassis , PartType.FrontChassis},
            { bumperRear , PartType.RearBumper },
            { rearChassis , PartType.RearChassis},
            { motor , PartType.Motor},
            { motorCarb , PartType.MotorCarb},
            { motorExhaustRight , PartType.MotorExhaustRight},
            { motorExhaustLeft , PartType.MotorExhaustLeft}
        };

        scrPartsToCacheName = new Dictionary<ScrParts, string>
        {
            { frontChassis ,   "_cachedFrontChassisName"},
            { bumperFront , "_cachedFrontBumperName"},
            { motor , "_cachedMotorName" },
            { motorCarb ,"_cachedMotorCarbName"},
            { motorExhaustLeft , "_cachedMotorExhaustLeftName" } ,
            { motorExhaustRight , "_cachedMotorExhaustRightName" }
        };


    }

    void OnEnable()
    {
        InitAllParts(false);
        InitWheelNonVisuals();

        for (int i = 0; i < 4; i++)
        {
            currentWheelPrefabs.Add(GetSocket(tireSocketNames[i]).GetChild(0).gameObject);
        }

        CustomizationManager.onPartChanged += PartChangeListener;
    }

    void OnDisable()
    {
        CustomizationManager.onPartChanged -= PartChangeListener;
    }

    private void Start()
    {
        //LoadTest();
        CachePartsByName();
    }

    private void CachePartsByName()
    {
        _cachedFrontChassisName = GetPart(PartType.FrontChassis)?.currentPart.name;
        _cachedFrontBumperName = GetPart(PartType.FrontBumper)?.currentPart.name;
        _cachedMotorName = GetPart(PartType.Motor)?.currentPart.name;
        _cachedMotorCarbName = GetPart(PartType.MotorCarb)?.currentPart.name;
        _cachedMotorExhaustLeftName = GetPart(PartType.MotorExhaustLeft)?.currentPart.name;
        _cachedMotorExhaustRightName = GetPart(PartType.MotorExhaustRight)?.currentPart.name;
    }

    public Transform GetSocket(string socketName)
    {
        Transform _socketTransform = null;
        for (int i = 0; i < sockets.Count; i++)
        {
            if (sockets[i].name == (socketName))
            {
                _socketTransform = sockets[i];
            }
        }
        return _socketTransform;
    }

    public static GameObject GetGameObject(string objName, ScrParts objType)
    {
        GameObject[] _objArray = objType.partPrefabs;

        for (int i = 0; i < _objArray.Length; i++)
        {
            if (_objArray[i].name == objName)
            {
                GameObject  _gameObject = objType.PartDatas[i].partGameObject;
                return _gameObject;
            }
        }

        Debug.LogError("Object is null");
        return null;
    }

    /*
    public void InitAllSockets()
    {      
        Transform[] allChilds = Sockets.GetComponentsInChildren<Transform>();
        for (int i2 = 0; i2 < allChilds.Length; i2++)
        {
            if (allChilds[i2].name == socketNames[i2])
            {
                sockets.Add(allChilds[i2]);
            }
        }
    }
    */

    public void InitAllSockets2()
    {
        sockets.Clear();

        Transform[] allChilds = Sockets.GetComponentsInChildren<Transform>();
        for (int i = 0; i < allChilds.Length; i++)
        {
            if (allChilds[i].name.Contains("SOCKET"))
            {
                allChilds[i].AddComponent<Socket_Validator>();
                sockets.Add(allChilds[i]);
            }        
        }
    }

    public void InitSocket(string socketName)
    {
        Transform[] allChilds = Sockets.GetComponentsInChildren<Transform>();

        for (int i = 0; i < allChilds.Length; i++)
        {
            if (allChilds[i].name == socketName)
            {
                sockets.Add(allChilds[i]);
            }
        }
    }

    public void InitAllParts(bool clearList)
    {
        if (clearList) { parts.Clear(); };

        InitParts(bonnet);
        InitParts(frontChassis);
        InitParts(rearChassis);
        InitParts(bumperFront);
        InitParts(bumperRear);
        InitParts(motor);
        InitParts(motorCarb);
        InitParts(motorExhaustLeft);
        InitParts(motorExhaustRight);
    }


    /// <summary>
    /// Initializing current instaled car parts
    /// </summary>
    /// <param name="scrParts"></param>
    public void InitParts(ScrParts scrParts)
    {
        Transform[] transforms = Sockets.transform.GetComponentsInChildren<Transform>();

        for (int i = 0; i < scrParts.partPrefabs.Length; i++)
        {
            foreach (Transform item in transforms)
            {
                if (item.name == scrParts.partPrefabs[i].name)
                {
                    Part _part = new()
                    {
                        socket = item.parent,
                        currentPartName = item.gameObject.name,
                        socketName = item.parent.name,
                        currentPart = item.gameObject
                    };

                    string partName = _part.currentPart.name;

                    if (partName.Contains("Bonnet"))
                    {
                        _part.partType = PartType.Bonnet;
                    }
                    else if (partName.Contains("Chassis") && partName.Contains("Front"))
                    {
                        _part.partType = PartType.FrontChassis;
                    }
                    else if (partName.Contains("Chassis") && partName.Contains("Rear"))
                    {
                        _part.partType = PartType.RearChassis;
                    }
                    else if (partName.Contains("Bumper") && partName.Contains("Front"))
                    {
                        _part.partType = PartType.FrontBumper;
                    }
                    else if (partName.Contains("Bumper") && partName.Contains("Rear"))
                    {
                        _part.partType = PartType.RearBumper;
                    }
                    else if (partName.Contains("Motor"))
                    {
                        if (partName.Contains("Carb"))
                        {
                            _part.partType = PartType.MotorCarb;
                        }
                        else if (partName.Contains("Exhaust"))
                        {
                            if (partName.Contains("Left"))
                            {
                                _part.partType = PartType.MotorExhaustLeft;
                            }
                            else if (partName.Contains("Right"))
                            {
                                _part.partType = PartType.MotorExhaustRight;
                            }
                        }
                        else
                        {
                            _part.partType = PartType.Motor;
                        }
                    }

                    parts.Add(_part);
                }
            }
        }
    }

    private void PartChangeListener(ScrParts scrParts, bool isNext, PartType partType)
    {
        InitAllSockets2();

        if (scrParts != tires)
        {
            ChangePart(scrParts, isNext, partType);
        }
        else
        {
            ChangeTire(isNext);
        }
    }

    private void ChangePart(ScrParts scrParts, bool isNext, PartType partType)
    {
        GameObject _prefab;
        GameObject _gameObject;

        Transform _partSocket = GetSocket(scrParts.socketName);


        switch (isNext)
        {
            case true:
                _prefab = scrParts.GetNextPart();
                break;

            case false:
                _prefab = scrParts.GetPrevPart();
                break;
        }

        if (GetPart(partType) == null)
        {
            Part _part = CreatePart(_partSocket, _partSocket.name, _prefab, _prefab.name, partType);
            parts.Add(_part);
        }
        else
        {
            Destroy(GetPart(partType).currentPart);
            InitAllSockets2();
        }

        _gameObject = CreateGameobject(_prefab, _partSocket.position, _partSocket.rotation, _partSocket);

        GetPart(partType).currentPart = _gameObject;
        GetPart(partType).currentPartName = _gameObject.name;

        if (scrParts == frontChassis || scrParts == rearChassis)
        {
            InitWheelNonVisuals();
            CustomizationManager.onChassisChanged.Invoke();
        }


        if (scrParts == bonnet)
        {
            InitAllSockets2();
            CreateWholePart(frontChassis, _cachedFrontChassisName);

            /*
            CreateFrontChassis();
            if grilled CreateMotor() , CreateMotorCarb() , CreateExhausts();
            */

          
            InitWheelNonVisuals();
            CustomizationManager.onChassisChanged.Invoke();
        }


        if (scrParts == motor
            || scrParts == motorCarb
            || scrParts == motorExhaustLeft
            || scrParts == motorExhaustRight
            || scrParts == bumperFront
            || scrParts == frontChassis
            || scrParts == rearChassis)
        {
            CachePartByName(partTypeToScrParts[scrParts], _gameObject.name);
        }


        if (scrParts == motor && _cachedMotorCarbName != null)
        {
            InitSocket(motor.socketName);
            InitSocket(motorCarb.socketName);
            CreateWholePart(motorCarb, _cachedMotorCarbName);
        }

        if (scrParts == bonnet && _cachedMotorName != null)
        {
            ENUMS.BonnetTypes type = bonnet.Get_BonnetType();
            CreateMotorSystems(type);         
        }

        if (scrParts == bonnet)
        {
            Debug.Log("cuhhhhhhhhhhhhh");
            foreach (var sh in scrParts.partPrefabs)
            {
                Debug.Log(sh.gameObject.name);
            }
            //bool type = scrParts.get
            //Debug.Log(type.ToString());
        }
    }


    private void CreateMotorSystems(ENUMS.BonnetTypes type) 
    {
        switch (type)
        {
            case BonnetTypes.Default:
                Debug.Log("Default Bonnet Type Motor Creation Stopped");
                break;
            case BonnetTypes.Grill:
                CreateWholePart(motor, _cachedMotorName);

                InitSocket(motorCarb.socketName);
                CreateWholePart(motorCarb, _cachedMotorCarbName);

                InitSocket(motorExhaustLeft.socketName);
                CreateWholePart(motorExhaustLeft, _cachedMotorExhaustLeftName);

                InitSocket(motorExhaustRight.socketName);
                CreateWholePart(motorExhaustRight, _cachedMotorExhaustRightName);
                break;
            case BonnetTypes.Hole:
                CreateWholePart(motor, _cachedMotorName);

                InitSocket(motorExhaustLeft.socketName);
                CreateWholePart(motorExhaustLeft, _cachedMotorExhaustLeftName);

                InitSocket(motorExhaustRight.socketName);
                CreateWholePart(motorExhaustRight, _cachedMotorExhaustRightName);
                break;
        }

       
    }
    private void CreateWholePart(ScrParts scrPart, string cachedName)
    {
       
        Transform _partSocket = GetSocket(scrPart.socketName);
        GameObject _prefab = scrPart.GetGameobjectByName(cachedName);
        GameObject _gameObject = CreateGameobject(_prefab.gameObject, _partSocket.position, _partSocket.rotation, _partSocket);
        GetPart(scrPart.partType).currentPart = _gameObject;
    }


    void CachePartByName(PartType partType, string partName)
    {
        switch (partType)
        {
            case PartType.FrontChassis:
                _cachedFrontChassisName = partName;
                break;

            case PartType.FrontBumper:
                _cachedFrontBumperName = partName;
                break;

            case PartType.Motor:
                _cachedMotorName = partName;
                break;

            case PartType.MotorCarb:
                _cachedMotorCarbName = partName;
                break;

            case PartType.MotorExhaustLeft:
                _cachedMotorExhaustLeftName = partName;
                break;

            case PartType.MotorExhaustRight:
                _cachedMotorExhaustRightName = partName;
                break;
        }
    }

    private void ChangeTire(bool isNext/*,TiresToChange tiresToChange*/)
    {
        GameObject _prefab = null;
        GameObject _gameObject = null;

        List<Transform> _tireSockets = new()
        {
            GetSocket(tireSocketNames[0]),
            GetSocket(tireSocketNames[1]),
            GetSocket(tireSocketNames[2]),
            GetSocket(tireSocketNames[3])
        };

        for (int i = 0; i < _tireSockets.Count; i++)
        {
            //Debug.Log(GetSocket(tireSocketNames[i].ToString()));
        }

        /*
        if (tiresToChange == TiresToChange.Front)
        {
            Transform FRSocket = AutoParts.GetSocket(tireSocketNames[0]);
            Transform FLSocket = AutoParts.GetSocket(tireSocketNames[1]);
            _tireSockets.Add(FRSocket);
            _tireSockets.Add(FLSocket);
        }
        else if (tiresToChange == TiresToChange.Rear)
        {
            Transform RRSocket = AutoParts.GetSocket(tireSocketNames[2]);
            Transform RLSocket = AutoParts.GetSocket(tireSocketNames[3]);
            _tireSockets.Add(RRSocket);
            _tireSockets.Add(RLSocket);
        }
        */

        switch (isNext)
        {
            case true:
                _prefab = tires.GetNextPart();
                break;

            case false:
                _prefab = tires.GetPrevPart();
                break;
        }


        for (int i = 0; i < _tireSockets.Count; i++)
        {
            Transform _tireSocket = _tireSockets[i];
            _gameObject = Instantiate(_prefab, _tireSocket.position, _tireSocket.rotation, _tireSocket);
            //_gameObject.transform.SetParent(autoController.wheels[i].wheelUAPI.transform);
        }

        for (int i = 0; i < _tireSockets.Count; i++)
        {
            var wheel = _gameObject.GetComponent<AutomobileWheelUtility>();
            //var wheelUAPI = autoController.wheels[i].wheelUAPI;
            //WheelController wheelUAPI = new WheelController();


            //wheelUAPI.WheelVisual = wheel.wheelCenter;
            //wheelUAPI.Width = wheel.wheelWidht;
            //wheelUAPI.Radius = wheel.wheelRadius;
            //wheelUAPI.GetComponent<WheelController>().wheel.Initialize(wheelUAPI.GetComponent<WheelController>());
            //wheelUAPI.GetComponent<WheelController>().wheel.offset == xyz


            /*
            Transform visualTransform = wheelUAPI.transform;
            if (visualTransform.parent != wheelUAPI.transform)
            {
                visualTransform.SetParent(wheelUAPI.transform);
            }
            */
        }

        foreach (var item in currentWheelPrefabs)
        {
            Destroy(item);
        }
    }

    private GameObject CreateGameobject(GameObject _prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        GameObject _gameObject = Instantiate(_prefab, position, rotation, parent);
        return _gameObject;
    }

    public void InitWheelNonVisuals()
    {
        wheelNonVisuals.Clear();

        wheelNonVisuals.Add(GetObjectFromChild("Wheel_Front_Right", transform));
        wheelNonVisuals.Add(GetObjectFromChild("Wheel_Front_Left", transform));
        wheelNonVisuals.Add(GetObjectFromChild("Wheel_Rear_Right", transform));
        wheelNonVisuals.Add(GetObjectFromChild("Wheel_Rear_Left", transform));
    }

    public static List<GameObject> GetWheelNonVisuals()
    {
        return wheelNonVisuals;
    }

    private Part GetPart(PartType _partType)
    {
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].partType == _partType)
            {
                return parts[i];
            }
        }
        Debug.Log("part is null");
        return null;
    }

    private Part CreatePart(Transform _socket, string _socketName, GameObject _currentPart, string _currentPartName, PartType _partType)
    {
        Part part = new()
        {
            partType = _partType,
            currentPart = _currentPart,
            socket = _socket,
            socketName = _socketName,
            currentPartName = _currentPartName
        };
        return part;
    }

    private GameObject GetObjectFromChild(string objName, Transform tr)
    {
        GameObject _gameObject = null;
        Transform[] t = tr.GetComponentsInChildren<Transform>();

        for (int i = 0; i < t.Length; i++)
        {
            if (t[i].name == objName)
            {
                _gameObject = t[i].gameObject;
            }
        }
        return _gameObject;
    }

    public void SaveTest()
    {
        SaveGame.Save(this.gameObject.name + "-parts", parts);
    }

    public void LoadTest()
    {
        string identifier = this.gameObject.name + "-parts";
        if (!SaveGame.Exists(identifier))
        {
            Debug.Log("Save Does Not Exists");
            return;
        }

        List<Part> _parts = new();
        _parts = SaveGame.Load<List<Part>>(identifier, parts);

        foreach (var item in parts)
        {
            //Socket.clear[parts.bağlantılısoketler]
            Destroy(item.currentPart);
        }

        parts = _parts;

        for (int i = 0; i < parts.Count; i++)
        {
            Part item = parts[i];
            string result = null;

            if (item.currentPartName.Contains("Clone"))
            {
                result = item.currentPartName.Substring(0, item.currentPartName.Length - 7);
            }
            else
            {
                result = item.currentPartName;
            }

            ScrParts scr = scrPartsToType[item.partType];
            Transform socket = GetSocket(scr.socketName);

            GameObject _gameObject = CreateGameobject(GetGameObject(result, scr), socket.position, socket.rotation, socket);
            GetPart(item.partType).currentPart = _gameObject;
            GetPart(item.partType).currentPartName = _gameObject.name;

            //this works 2 time consider changes for optimization
            if (scr == frontChassis || scr == rearChassis)
            {
                InitAllSockets2();
                InitWheelNonVisuals();
                CustomizationManager.onChassisChanged.Invoke();
            }

            if (scr == bonnet || scr == motor)
            {
                InitAllSockets2();
            }
        }
        CachePartsByName();

        InitAllSockets2();

        //ClearMissingSockets();
        //DevKit.ClearMissingsFromList(ref sockets);
    }

    public void ClearMissingSockets()
    {
        /// İŞE YARAMIYOR
        DevKit.ClearMissingsFromList(ref sockets);
    }



    public void Reset()
    {
        foreach (Transform transform in transform)
        {
            if (transform.gameObject.name == "SOCKETS")
            {
                Sockets = transform;
            }
        }
       
        InitAllSockets2();

        //Syncronous adressables loading
        var op0 = Addressables.LoadAssetAsync<ScrPartsBonnet>("Bonnets");
        bonnet = op0.WaitForCompletion();     
        var op1 = Addressables.LoadAssetAsync<ScrParts>("Front Chassis");
        frontChassis = op1.WaitForCompletion();
        var op2 =  Addressables.LoadAssetAsync<ScrParts>("Rear Chassis");
        rearChassis = op2.WaitForCompletion();
        var op3 = Addressables.LoadAssetAsync<ScrParts>("Front Bumper");
        bumperFront = op3.WaitForCompletion();
        var op4 =Addressables.LoadAssetAsync<ScrParts>("Rear Bumper");
        bumperRear = op4.WaitForCompletion();
        var op5    = Addressables.LoadAssetAsync<ScrParts>("Tires");
        tires = op5.WaitForCompletion();
        var op6 =  Addressables.LoadAssetAsync<ScrParts>("Motor");
        motor = op6.WaitForCompletion();
        var op7 =Addressables.LoadAssetAsync<ScrParts>("Motor Carb");
        motorCarb = op7.WaitForCompletion();
        var op8 = Addressables.LoadAssetAsync<ScrParts>("Motor Exhaust Left");
        motorExhaustLeft = op8.WaitForCompletion();
        var op9 =Addressables.LoadAssetAsync<ScrParts>("Motor Exhaust Right");
        motorExhaustRight = op9.WaitForCompletion();
        //

        autoController = GetComponent<AutoController>();  
    }

    /*
     public void DeletePreviousPart(GameObject part)
    {
        for (int i = 0; i  < parts.Count; i ++)
        {
           if (parts[i].currentPart.name == part.name) { Destroy(part); }
        }
    }
    */

}
