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
		public int uId = -1;//This object's uid
		public int ownedByClId = -1;//The uId of the peer that owns this object
		public int attachedTo = -1;//The uid of the object this object is attached to
		public Vector2 gridPos = new Vector2(-999, -999);//These contain the x/y ints if this object is attached to a grid of another object
		public bool selected = false;
		public bool isUnit = false;
		public float stopDist = 1;
		public TextMesh textMesh;
		public float turnSpd = 55;
		public float spd = 5;
		public bool gridVisible = false;
		public GridSq[,] grid;
		public float gridObjAnglesOffset = 0;//When aligning with a possible grid to be dropped on, offset Y angle by this much
		public float desiredAngle = 0;
		public float range = 0;
		public float firingConeAngle = 0;//doubled, so 15 is a 30 degree cone

		public int[] visualState = new int[3];
		public int[] oldVisualState = new int[3];
		public Renderer[] mainBodyRenderers;
		/*
		 * 
		 */
		public void UpdateObjVisualState()
		{
			//This function is called every frame on the client, and changes the materials/colors/shaders of the object,
			//based on what's happening to it. (example, it's behind terrain, out of the fog of war & being dragged).

			//Has the visual state changed?
			bool update = false;
			for (int i = 0; i < visualState.Length; i++)
			{
				if (visualState[i] != oldVisualState[i]) { update = true; break; }
			}

			if (update)
			{
				for (int i = 0; i < mainBodyRenderers.Length; i++)
				{
					if (visualState[0] == 0)//Default visual state
					{
						mainBodyRenderers[i].material = xa.de.defaultHullMat;

					}
					else if (visualState[0] == 1)//Being dragged, over a valid sq
					{
						mainBodyRenderers[i].material = xa.de.validHullMat;
					}
				}
			}

			//sync the states
			for (int i = 0; i < visualState.Length; i++)
			{
				oldVisualState[i] = visualState[i];
			}
		}
	}

	public class GridSq
	{
		public List<int> objs;//a list of all uIds that are on this grid sq
		public GameObject go;//the game object of the gridSq (a small icon or outline)
		public bool valid = false;//True if this is a valid sq
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
				obj.go.GetComponent<Info>().uId = obj.uId;
				obj.mainBodyRenderers = obj.go.GetComponent<Info>().mainBodyRenderers;

				InitGrid(obj);
				CreateGridSqPrefabs(obj);
				Vector2 gridPos = GlobalPosToGridXY(obj, obj.go.transform.position);
				obj.gridPos = gridPos;
				//Debug.Log("GridPos: " + gridPos);
				RegisterOnGrid(obj, obj, gridPos); //Register this unit on its own grid
				TurnOffGrid(obj);//Turns off this grid, since it was probably created as visible.
								 //TurnOnAllValidGridSquares(obj);
				break;
			case Type.HullBlock:
				obj.go.GetComponent<Info>().uId = obj.uId;
				obj.mainBodyRenderers = obj.go.GetComponent<Info>().mainBodyRenderers;
				obj.textMesh = obj.go.GetComponentInChildren<TextMesh>();
				obj.textMesh.text = "gridXY";
				break;
			case Type.Engine:
				obj.go.GetComponent<Info>().uId = obj.uId;
				obj.mainBodyRenderers = obj.go.GetComponent<Info>().mainBodyRenderers;
				obj.textMesh = obj.go.GetComponentInChildren<TextMesh>();
				obj.textMesh.text = "gridXY";
				break;
			case Type.Gun:
				obj.go.GetComponent<Info>().uId = obj.uId;
				obj.mainBodyRenderers = obj.go.GetComponent<Info>().mainBodyRenderers;
				obj.textMesh = obj.go.GetComponentInChildren<TextMesh>();
				obj.textMesh.text = "gridXY";
				obj.range = 15;
				obj.firingConeAngle = 15;
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
		HandleCombat();

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

		for (int i = 0; i < objs.Count; i++)
		{
			objs[i].UpdateObjVisualState();
		}

	}

	public static void HandleCombat()
	{
		
		//draw firing cones from guns
		for (int i = 0; i < objs.Count; i++)
		{
			Obj o = objs[i];
			if (o.range > 0 && o.attachedTo != -1)//Is a gun, and is attached to a unit
			{
				Vector3 origin;
				Vector3 ang;
				Vector3 leftPos;
				Vector3 rightPos;
				Vector3 centerPos;

				origin = o.go.transform.position;
				ang = o.go.transform.eulerAngles;
				centerPos = MathFuncs.ProjectVec(origin, ang, o.range, Vector3.forward);
				leftPos = MathFuncs.ProjectVec(origin, new Vector3(ang.x,ang.y + o.firingConeAngle,ang.z), o.range, Vector3.forward);
				rightPos = MathFuncs.ProjectVec(origin, new Vector3(ang.x,ang.y - o.firingConeAngle,ang.z), o.range, Vector3.forward);
				Effects.DrawLine(origin,leftPos,0.1f, Effects.Colors.White);
				Effects.DrawLine(leftPos,centerPos,0.1f, Effects.Colors.White);
				Effects.DrawLine(centerPos,rightPos,0.1f, Effects.Colors.White);
				Effects.DrawLine(rightPos,origin,0.1f, Effects.Colors.White);
			}
		}
	}

	public static void MoveUnits()
	{
		//Move & turn units
		for (int i = 0; i < objs.Count; i++)
		{
			Obj o = objs[i];
			if (o.isUnit)
			{
				//Debug.DrawLine(o.go.transform.position,o.goal, Color.green);
				Effects.DrawLine(o.go.transform.position, o.goal, 0.2f, Effects.Colors.Red);

				//Is the unit far enough away from it's goal, that it should go there?
				if (Vector3.Distance(o.go.transform.position, o.goal) > o.stopDist)
				{
					//Slow turn at a flat rate
					Vector3 pos = o.go.transform.position;
					float tempSpd = o.spd;

					o.go.transform.rotation = Setup.SlowTurn(pos, o.go.transform.rotation, o.goal, o.turnSpd);

					//Is the unit pointing at it's goal?
					if (MathFuncs.CheckCone(-1, 15, o.go.transform.localEulerAngles.y, o.go.transform.position, o.goal, true))
					{
						Effects.DrawLine(new Vector3(pos.x, pos.y + 1, pos.z), new Vector3(o.goal.x, o.goal.y + 1, o.goal.z), 0.3f, Effects.Colors.Cyan);
						tempSpd = o.spd;
					}
					else
					{
						//tempSpd = o.spd;// * 0.25f;//turning. Move at 1/4 speed
						tempSpd = 0;
					}

					o.go.transform.Translate(new Vector3(0, 0, tempSpd * Time.deltaTime));
				}
			}
		}
	}

	public static void InitGrid(Obj obj)
	{
		obj.grid = new GridSq[35, 35];
		for (int y = 0; y < obj.grid.GetLength(1); y++)
		{
			for (int x = 0; x < obj.grid.GetLength(0); x++)
			{
				obj.grid[x, y] = new GridSq();
				obj.grid[x, y].objs = new List<int>();
			}
		}
	}

	public static void CreateAllGrids()
	{
		for (int i = 0; i < objs.Count; i++)
		{
			TurnOnAllValidGridSquares(objs[i]);
		}
	}

	public static void CreateGridSqPrefabs(Obj obj)
	{
		int gridX = obj.grid.GetLength(0);
		int gridY = obj.grid.GetLength(1);
		for (int y = 0; y < gridY; y++)
		{
			for (int x = 0; x < gridX; x++)
			{
				Vector3 pos = new Vector3(0, 0, 0);
				Vector3 ang = new Vector3(0, 0, 0);
				pos = obj.go.transform.position;
				pos.x += (x - 17);
				pos.z += (y - 17);
				GameObject go = (GameObject)Instantiate(xa.de.gridSqPrefab, pos, Quaternion.Euler(ang.x, ang.y, ang.z));
				go.transform.parent = obj.go.transform;
				obj.grid[x, y].go = go;
			}
		}
	}

	public static void TurnOffGrid(Obj obj)
	{
		obj.gridVisible = false;
		int gridX = obj.grid.GetLength(0);
		int gridY = obj.grid.GetLength(1);
		for (int y = 0; y < gridY; y++)
		{
			for (int x = 0; x < gridX; x++)
			{
				obj.grid[x, y].go.SetActive(false);
				obj.grid[x, y].valid = false;
			}
		}
	}

	public static void TurnOffAllGrids()
	{
		for (int i = 0; i < objs.Count; i++)
		{
			if (objs[i].isUnit && objs[i].gridVisible)//Since we're looping through EVERY unit, let's check if this one's grid is visible or not before bothering to turn it off.
			{
				TurnOffGrid(objs[i]);
			}
		}
	}
	/*
	public static void TurnOnAllGridSquares(Obj obj)
	{
		obj.gridVisible = true;
		int gridX = obj.grid.GetLength(0);
		int gridY = obj.grid.GetLength(1);
		for (int a = 0; a < gridY; a++)
		{
			for (int b = 0; b < gridX; b++)
			{
				obj.grid[a, b].go.SetActive(true);
				obj.grid[x, y].valid = true;
			}
		}
	}
	*/
	public static void TurnOnAllValidGridSquares(Obj obj)
	{
		obj.gridVisible = true;
		int gridX = obj.grid.GetLength(0);
		int gridY = obj.grid.GetLength(1);
		for (int y = 0; y < gridY; y++)
		{
			for (int x = 0; x < gridX; x++)
			{
				if (!IsGridSqFull(obj, x, y))//Is it empty?
				{
					//Is it next to a not-empty tile? (later on, this will have to be based on PF from the center)
					if (IsGridSqFull(obj, x + 1, y) ||
						IsGridSqFull(obj, x - 1, y) ||
						IsGridSqFull(obj, x, y + 1) ||
						IsGridSqFull(obj, x, y - 1))
					{
						obj.grid[x, y].go.SetActive(true);
						obj.grid[x, y].valid = true;
					}
				}
			}
		}
	}

	public static bool IsOnGrid(Obj obj, Vector2 gridPos) { return IsOnGrid(obj, (int)gridPos.x, (int)gridPos.y); }//Returns true if empty or off-the-grid xy
	public static bool IsOnGrid(Obj gridObj, int x, int y)//Returns true if empty or off-the-grid xy
	{
		if (x < 0) { return false; }
		if (y < 0) { return false; }
		if (x >= gridObj.grid.GetLength(0)) { return false; }
		if (y >= gridObj.grid.GetLength(1)) { return false; }
		return true;
	}


	public static bool IsGridSqFull(Obj gridObj, Vector2 gridPos)//Returns true if empty or off-the-grid xy
	{ return IsGridSqFull(gridObj, (int)gridPos.x, (int)gridPos.y); }
	public static bool IsGridSqFull(Obj gridObj, int x, int y)//Returns true if empty or off-the-grid xy
	{
		if (!IsOnGrid(gridObj, x, y)) { return false; }//Off grid counts as empty
		if (gridObj.grid[x, y].objs.Count > 0)//Is it not empty?
		{
			return true;
		}
		return false;
	}

	public static bool IsGridSqValid(Obj gridObj, Vector2 gridPos)//Returns true if empty or off-the-grid xy
	{ return IsGridSqValid(gridObj, (int)gridPos.x, (int)gridPos.y); }
	public static bool IsGridSqValid(Obj gridObj, int x, int y)//Returns true if empty or off-the-grid xy
	{
		if (!IsOnGrid(gridObj, x, y)) { return false; }//Off grid counts as not-valid
		if (gridObj.grid[x, y].valid)//Is it a valid gridSq?
		{
			return true;
		}
		return false;
	}

	public static void RegisterOnGrid(Obj gridObj, Obj obj, Vector2 gridPos)
	{
		RegisterOnGrid(gridObj, obj, (int)gridPos.x, (int)gridPos.y);
	}
	public static void RegisterOnGrid(Obj gridObj, Obj obj, int x, int y)
	{
		gridObj.grid[x, y].objs.Add(obj.uId);
	}

	public static Vector2 GlobalPosToGridXY(Obj gridObj, Vector3 pos)
	{
		//Take the world postion, and turn into to local coords of the gridObj
		Vector3 localPos = gridObj.go.transform.InverseTransformPoint(pos);

		//Take that pos, and add half the grid size, rounded down (as it's an odd number, so cut off the .5f)
		localPos.x += Mathf.FloorToInt(gridObj.grid.GetLength(0) * 0.5f);
		localPos.z += Mathf.FloorToInt(gridObj.grid.GetLength(1) * 0.5f);

		localPos.x = Mathf.RoundToInt(localPos.x);//Flatten to ints
		localPos.z = Mathf.RoundToInt(localPos.z);

		//this should be a grid index
		Vector2 result = new Vector2(localPos.x, localPos.z);//Return the x/z coords as x/y
		return result;
	}

	public static Vector3 GridXYToLocalPos(Obj gridObj, Vector2 gridPos)//gridObj means the obj which has the grid
	{
		//Take that pos, and subtract half the grid size, rounded down (as it's an odd number, so cut off the .5f)
		gridPos.x -= Mathf.FloorToInt(gridObj.grid.GetLength(0) * 0.5f);
		gridPos.y -= Mathf.FloorToInt(gridObj.grid.GetLength(1) * 0.5f);

		//this should be a local pos
		Vector3 result = new Vector3(gridPos.x, 0, gridPos.y);//Return the x/y coords as x/z
		return result;
	}

	//Happens on the client, dragging is a local function
	public static void PickingUpAnObj(int uId)
	{
		Cl_Orders.gridObjs = new List<ObjFuncs.Obj>();
		int index = GetObjIndexForUID(uId);
		if (objs[index] == null) { return; }

		//Make a list of grids that should be checked against.
		for (int i = 0; i < objs.Count; i++)
		{
			if (objs[i].isUnit && objs[i].ownedByClId == hl.local_uId)//Make sure it's a unit & belongs to this client
			{
				float dist = 20;//Only display the grids of units within this distance
				if (Setup.DistXZ(objs[i].go.transform.position, objs[index].go.transform.position) < dist)
				{
					Cl_Orders.gridObjs.Add(objs[i]);
				}
			}
		}

		//Turn those grids on
		for (int i = 0; i < Cl_Orders.gridObjs.Count; i++)
		{
			TurnOnAllValidGridSquares(Cl_Orders.gridObjs[i]);
		}
	}

	public static int IfOverValidPos(int uId)
	{
		int index = GetObjIndexForUID(uId);
		if (objs[index] == null) { return -1; }

		//Is this object on any of the grids?
		for (int i = 0; i < Cl_Orders.gridObjs.Count; i++)
		{
			Vector2 gridPos = GlobalPosToGridXY(Cl_Orders.gridObjs[i], objs[index].go.transform.position);
			if (IsOnGrid(Cl_Orders.gridObjs[i], gridPos))
			{
				objs[index].textMesh.text = "Grid x: " + gridPos.x + ", y: " + gridPos.y;
				//Maybe it can be dropped on this grid
				if (IsGridSqValid(Cl_Orders.gridObjs[i], gridPos))
				{
					//It can be! Yay!
					return Cl_Orders.gridObjs[i].uId;
				}
			}
		}



		return -1;
	}

	//Happens on the client, dragging is a local function
	public static void DroppingAnObj(int uId)
	{
		int index = GetObjIndexForUID(uId);
		if (objs[index] == null) { return; }

		//Is this object on any of the grids?
		for (int i = 0; i < Cl_Orders.gridObjs.Count; i++)
		{
			Vector2 gridPos = GlobalPosToGridXY(Cl_Orders.gridObjs[i], objs[index].go.transform.position);
			if (IsOnGrid(Cl_Orders.gridObjs[i], gridPos))
			{
				//Maybe it can be dropped on this grid
				if (IsGridSqValid(Cl_Orders.gridObjs[i], gridPos))
				{
					//It can be! Yay!
					Debug.Log("VALID DROP POINT");
					Obj o = objs[index];
					Obj gridObj = Cl_Orders.gridObjs[i];

					Effects.Bubbles(o.go.transform.position);

					//attach it!
					o.visualState[0] = 0;
					o.gridPos = gridPos;
					Vector3 localPos = GridXYToLocalPos(gridObj, gridPos);
					Debug.Log("GridPos: " + gridPos + ", LocalPos: " + localPos);
					o.attachedTo = gridObj.uId;
					o.go.transform.parent = gridObj.go.transform;
					o.go.transform.LocalSetX(localPos.x);
					o.go.transform.LocalSetZ(localPos.z);//Snap to this point's position
														 //o.go.transform.SetAngY(gridObj.go.transform.localEulerAngles.y);

					RegisterOnGrid(gridObj, o, gridPos); //Register this unit on its own grid
					break;//Break out, as we only want this object to attach to one of the grids.
				}
			}
		}

		//Turn off all grids
		TurnOffAllGrids();
	}

	public static int GetObjIndexForUID(int uId)
	{
		for (int i = 0; i < objs.Count; i++)
		{
			if (objs[i].uId == uId) { return i; }
		}
		return -1;
	}



}
