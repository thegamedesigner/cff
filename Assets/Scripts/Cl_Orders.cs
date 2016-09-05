using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cl_Orders : MonoBehaviour
{
	public enum MouseOrders { None, SelectionBox_StartDragging, SelectionBox_StopDragging, DraggingObj_Start, DraggingObj_Stop, End }
	public static int draggingObj = -1;
	public static List<ObjFuncs.Obj> gridObjs;//Objs that the dragged object is checking against their grids

	public static void InputOrders()//Called every frame from hl.HlObj Update
	{
		//There is a generic way to do this, but for now, I'm just going to handle each case

		//Make sure that the connection isn't null
		if (hl.hlObj == null) { return; }

		//Get mouse pos in the world
		Vector3 mousePos = Vector3.zero;
		Ray ray = xa.de.globalMainCam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 999, xa.de.toHitFloorMask))
		{
			mousePos = hit.point;
		}

		MouseOrders mouseOrder = MouseOrders.None;
		if (Input.GetMouseButtonDown(0))//Left click - Drag selection or drag object?
		{
			//is the mouse over a object that can be dragged?
			draggingObj = -1;
			ray = xa.de.globalMainCam.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit, 999, xa.de.toHitDraggableObjs))
			{
				//An obj is under the mouse, 
				int uId = hit.collider.gameObject.GetComponent<HitboxScript>().go.GetComponent<Info>().uId;
				int index = ObjFuncs.GetObjIndexForUID(uId);

				//can it be dragged?
				if (!ObjFuncs.objs[index].isUnit && ObjFuncs.objs[index].attachedTo == -1)
				{
					mouseOrder = MouseOrders.DraggingObj_Start;
					draggingObj = uId;
					ObjFuncs.PickingUpAnObj(draggingObj);
				}


			}


			if(draggingObj == -1)
			{
				//No object under my mouse, so start dragging a selection box
				mouseOrder = MouseOrders.SelectionBox_StartDragging;
			}
		}
		if (Input.GetMouseButtonUp(0))//Left click up, release dragged object or stop selection box?
		{
			if (draggingObj != -1)//I was dragging an obj. Drop it. 
			{
				//Dropping an object
				mouseOrder = MouseOrders.DraggingObj_Stop;

				ObjFuncs.DroppingAnObj(draggingObj);

				draggingObj = -1;
			}
			else
			{
				//I wasn't dragging an obj, so I must have been dragging a selection box
				mouseOrder = MouseOrders.SelectionBox_StopDragging;
			}
		}
		if (draggingObj != -1)//Dragging an object
		{
			int index = ObjFuncs.GetObjIndexForUID(draggingObj);
			ObjFuncs.objs[index].go.transform.SetX(mousePos.x);
			ObjFuncs.objs[index].go.transform.SetZ(mousePos.z);

			ObjFuncs.objs[index].visualState[0] = 0;
			int result = ObjFuncs.IfOverValidPos(draggingObj);
			if (result != -1)
			{
				//change color
				ObjFuncs.objs[index].visualState[0] = 1;

				//Align to the possible gridObj's angle
				int gridObjIndex = ObjFuncs.GetObjIndexForUID(result);
				ObjFuncs.objs[index].desiredAngle = ObjFuncs.objs[gridObjIndex].go.transform.localEulerAngles.y;


			}

			//snap to this (tween later)
			ObjFuncs.objs[index].go.transform.SetAngY(ObjFuncs.objs[index].desiredAngle + ObjFuncs.objs[index].gridObjAnglesOffset);

			//detect if the player wants to rotate this object
			if (Input.GetKeyDown(KeyCode.Q))//Rotate the object
			{
				ObjFuncs.objs[index].gridObjAnglesOffset -= 90;
			}
			if (Input.GetKeyDown(KeyCode.E))//Rotate the object
			{
				ObjFuncs.objs[index].gridObjAnglesOffset += 90;
			}
		}

		SelectionScript.HandleSelection(mouseOrder);//Handles local selection box

		//Issue a move command to all selected units of the correct player
		if (Input.GetMouseButtonDown(1))//Move order
		{
			//Send the order to the server. Fire and forget!
			ObjFuncs.Obj o;

			//send one order per unit ordered.
			List<int> result = new List<int>();
			for (int i = 0; i < ObjFuncs.objs.Count; i++)
			{
				o = ObjFuncs.objs[i];
				if (o.isUnit && o.ownedByClId == hl.local_uId && o.selected)
				{
					result.Add(o.uId);
				}
			}
			int[] unitIds = new int[result.Count];
			for (int i = 0; i < result.Count; i++) { unitIds[i] = result[i]; }//write to the array

			hl.hlObj.CmdMoveOrder(hl.local_uId, mousePos, unitIds);

			//Trigger local effect
			Effects.MoveOrderEffect(mousePos);
		}

		//This is currently hardwired to specific keys, but later players will be able to rebind.
		if (Input.GetKeyDown(KeyCode.Alpha1))//Create a command pod!
		{
			ChatScript.ChatLocally("Detected local createObj order: Command Pod");
			//Send the order to the server
			hl.hlObj.CmdCreateObj(hl.local_uId, mousePos, (int)ObjFuncs.Type.CommandPod);

			//Trigger local effect
			//Effects.CircleBlip(mousePos, 5, 5);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2))//Create a hull block
		{
			ChatScript.ChatLocally("Detected local createObj order: Hull Block");
			//Send the order to the server
			hl.hlObj.CmdCreateObj(hl.local_uId, mousePos, (int)ObjFuncs.Type.HullBlock);

			//Trigger local effect
			//Effects.CircleBlip(mousePos, 5, 5);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3))//Create an engine
		{
			ChatScript.ChatLocally("Detected local createObj order: Engine");
			//Send the order to the server
			hl.hlObj.CmdCreateObj(hl.local_uId, mousePos, (int)ObjFuncs.Type.Engine);

			//Trigger local effect
			//Effects.CircleBlip(mousePos, 5, 5);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4))//Create a gun
		{
			ChatScript.ChatLocally("Detected local createObj order: Gun");
			//Send the order to the server
			hl.hlObj.CmdCreateObj(hl.local_uId, mousePos, (int)ObjFuncs.Type.Gun);

			//Trigger local effect
			//Effects.CircleBlip(mousePos, 5, 5);
		}
	}
}
