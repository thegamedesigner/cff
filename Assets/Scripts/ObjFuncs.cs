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

	public class Obj//Not a sendable class
	{
		//Sync'd variables (have to be added to Ticks.cs by hand, but seperated here for clarity)
		public Vector3 goal = Vector3.zero;

		public Type type;
		public GameObject go;
		public GameObject selectionCircle;
		public int uId = -1;
		public int ownedByClId = -1;
		public bool selected = false;
		public bool isUnit = false;
		public float stopDist = 1;
		public TextMesh textMesh;
		public float turnSpd = 155;
		public float spd = 25;
	}

	//Only EVER called on the server
	public static int GetUniqueId()
	{
		hl.uIds++;
		return hl.uIds;
	}

	public static void CreateObj(Type type, Vector3 pos, Vector3 ang, int clId, int uId)
	{
		//Create the go
		Obj obj = new Obj();
		obj.type = type;
		obj.go = (GameObject)Instantiate(PrefabLibrary.GetPrefabForType(type), pos, new Quaternion(0, 0, 0, 0));
		obj.go.transform.localEulerAngles = ang;
		obj.ownedByClId = clId;
		obj.uId = uId;
		obj.goal = pos;

		switch (type)
		{
			case Type.CommandPod:
				obj.isUnit = true;//This obj is a unit, and can be selected.
								  //create selectionCircle
				obj.selectionCircle = (GameObject)Instantiate(xa.de.selectionCirclePrefab, pos, new Quaternion(0, 0, 0, 0));
				obj.selectionCircle.transform.parent = obj.go.transform;
				obj.selectionCircle.transform.localPosition = Vector3.zero;
				obj.selectionCircle.SetActive(false);
				obj.textMesh = obj.go.GetComponentInChildren<TextMesh>();
				obj.textMesh.text = "uId: " + obj.uId + "\nClId: " + obj.ownedByClId;
				break;
		}

		objs.Add(obj);
	}

	public static void UpdateObjsOnServer()
	{
		//MoveUnits();//Move the units
	}

	public static void UpdateObjsLocally()
	{
		MoveUnits();//Move the units

		//Handle selection
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

	public static void MoveUnits()
	{
		//Move & turn units
		for (int i = 0; i < objs.Count; i++)
		{
			Obj o = objs[i];
			//Debug.DrawLine(o.go.transform.position,o.goal, Color.green);
			Effects.DrawLine(o.go.transform.position, o.goal, 3, Effects.Colors.Red);

			//Is the unit far enough away from it's goal, that it should go there?
			if (Vector3.Distance(o.go.transform.position, o.goal) > o.stopDist)
			{
				//Slow turn at a flat rate
				Vector3 pos = o.go.transform.position;

				o.go.transform.rotation = Setup.SlowTurn(pos, o.go.transform.rotation, o.goal, o.turnSpd);

				//Is the unit pointing at it's goal?
				if (MathFuncs.CheckCone(-1, 15, o.go.transform.localEulerAngles.y, o.go.transform.position, o.goal, true))
				{
					//move ahead slowly
					Effects.DrawLine(new Vector3(pos.x, pos.y + 5, pos.z), new Vector3(o.goal.x, o.goal.y - 10, o.goal.z), 4, Effects.Colors.Cyan);
					o.go.transform.Translate(new Vector3(0, 0, o.spd * Time.deltaTime));
				}
			}
		}
	}

	public static int GetObjIndexForUID(int uId)
	{
		for(int i = 0;i < objs.Count;i++)
		{
			if(objs[i].uId == uId) {return i; }
		}
		return -1;
	}

}
