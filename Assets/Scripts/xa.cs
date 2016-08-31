using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class xa : MonoBehaviour
{
    public static Main ma;
    public static Defines de;
    public static Setup se;
	public static Effects ef;
	public static PrefabLibrary pr;
	
    public static GameObject emptyObj = null;
    public static GameObject mainNodeObj = null;
	
	public static Vector3 lastGoal = Vector3.zero;
	public static Vector3 goal = Vector3.one;
}
