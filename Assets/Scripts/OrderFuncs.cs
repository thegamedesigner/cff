using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OrderFuncs : MonoBehaviour
{
	/* This script is for: */
	/* Collecting all dumb terminal commands on the client, 
	 * playing all local effects resulting from them. */
	 
	public static List<Order> savedOrders = new List<Order>();

	public enum Orders
	{
		None=0,
		CreateObj=1,
		MoveOrder=2,
		End
	}
	
	public class Order
	{
		public bool enacted = false;//Currently, nothing cleans up enacted orders. One of us should write something to fix this.
		public Orders order;
		public float timeStamp = -1;
		public int clId = -1;
		public Vector3 mousePos = Vector3.zero;
		public ObjFuncs.Type objType = ObjFuncs.Type.None;
	}

	//Called on the server, when an order is recieved by the server.
	public static void RecordOrder(int order, int clId, Vector3 mousePos, int objType)//Called in HlObj CmdSendInput
	{
		Order savedOrder = new Order();
		savedOrder.clId = clId;
		savedOrder.order = (Orders)order;
		savedOrder.timeStamp = Time.timeSinceLevelLoad;
		savedOrder.mousePos = mousePos;
		savedOrder.objType = (ObjFuncs.Type)objType;
		savedOrders.Add(savedOrder);
	}

	public static void CollectOrders()//Called every frame from ClientSideLoop update
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

		//This is currently hardwired to specific keys, but later players will be able to rebind.

		
		if (Input.GetKeyDown(KeyCode.Alpha1))//Create a command pod!
		{
			//Send the order to the server. Fire and forget!
			hl.hlObj.CmdSendOrder((int)Orders.CreateObj, hl.local_uId, mousePos, (int)ObjFuncs.Type.CommandPod);

			//Trigger local effect
			Effects.CircleBlip(mousePos, 5, 5);
		}
	}

}
