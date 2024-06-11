using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ENUMS
{
    public enum Socket
    {
        SOCKET_Attachment_Door_Right,
        SOCKET_Bonnet,
        SOCKET_Bumper_Rear,
        SOCKET_Char,
        SOCKET_Chassis_Rear,
        SOCKET_Fenders_Rear,
        SOCKET_Fenders_Step,
        SOCKET_Bumper_Front,
        SOCKET_Chassis_Front,
        SOCKET_Fenders_Front,
        SOCKET_Motor,
        SOCKET_Motor_Exhaust,
        SOCKET_Wheel_Spare,
        SOCKET_Wheel_Front_Left,
        SOCKET_Wheel_Front_Right,
        SOCKET_Wheel_Rear_Left,
        SOCKET_Wheel_Rear_Right
    }

    public enum PartType
    {
       None,
       Bonnet,
       FrontChassis,
       RearChassis,
       FrontBumper,
       RearBumper,
       Tire,
       Motor,
       MotorExhaustRight,
       MotorExhaustLeft,
       MotorCarb
    }

    public enum BonnetTypes 
    {
        Default,
        Grill,
        Hole
    }

    public enum TiresToChange 
    {
        Front,
        Rear
    }

}
