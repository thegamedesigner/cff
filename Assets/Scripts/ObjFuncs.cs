using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjFuncs : MonoBehaviour
{
	public static List<Obj> objs = new List<Obj>();

	public enum Type
	{
		None = 0,
		CommandPod = 1,
		HullBlock = 2,
		Gun = 3,
		Engine = 4,
		End
	}

	public class Obj
	{
		public Type type;
		public GameObject go;
		public GameObject selectionCircle;
		public int uId = -1;
		public bool selected = false;
		public bool isUnit = false;
	}

	//Only called on the server
	public static void CreateObj(Type type, Vector3 pos, Vector3 ang)
	{
		//Create the go
		Obj obj = new Obj();
		obj.type = type;
		obj.go = (GameObject)Instantiate(PrefabLibrary.GetPrefabForType(type), pos, new Quaternion(0, 0, 0, 0));
		obj.go.transform.localEulerAngles = ang;
		hl.uIds++;
		obj.uId = hl.uIds;

		switch (type)
		{
			case Type.CommandPod:
				obj.isUnit = true;//This obj is a unit, and can be selected.
				//create selectionCircle
				obj.selectionCircle = (GameObject)Instantiate(xa.de.selectionCirclePrefab, pos, new Quaternion(0, 0, 0, 0));
				obj.selectionCircle.transform.parent = obj.go.transform;
				obj.selectionCircle.transform.localPosition = Vector3.zero;
				obj.selectionCircle.SetActive(false);
				break;
		}

		objs.Add(obj);
	}

	public static void UpdateObjsOnServer()
	{

	}

	public static void UpdateObjsLocally()
	{
		//Flip through all objects, find which units are in the box
		for (int i = 0; i < objs.Count; i++)
		{
			Obj o = objs[i];
			//is it a unit?
			if (o.isUnit)
			{
				if (o.selected != o.selectionCircle.activeSelf)
				{
					o.selectionCircle.SetActive(o.selected);
				}
			}
		}

	}
}
