using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.PlayerSettings;

public static class DevKit
{
    public static void ClearMissingsFromList<T>(ref List<T> list)
    {
        list.RemoveAll(T => T == null);
    }
}
