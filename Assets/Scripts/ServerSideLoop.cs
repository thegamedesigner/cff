using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ServerSideLoop : MonoBehaviour
{
	public static void UpdateServer()//Called by the server's version of the network manager object
	{
		HandleSvGameplayLogic();
	}

	public static void HandleSvGameplayLogic()
	{
		ObjFuncs.UpdateObjsOnServer();

		//Loop through recorded dumb terminal commands and enact them.
		for (int i = 0; i < OrderFuncs.savedOrders.Count; i++)
		{
			if (!OrderFuncs.savedOrders[i].enacted)
			{
				if (OrderFuncs.savedOrders[i].timeStamp <= Time.timeSinceLevelLoad)
				{
					EnactOrder(OrderFuncs.savedOrders[i]);
					OrderFuncs.savedOrders[i].enacted = true;
				}
			}
		}

	}

	public static void EnactOrder(OrderFuncs.Order savedOrder)
	{
		//enact it
		switch (savedOrder.order)
		{
			case OrderFuncs.Orders.CreateObj:

				//Trigger local effect
				Effects.CircleBlip(savedOrder.mousePos, 5, 3);

				//Create the correct object of type
				ObjFuncs.CreateObj(savedOrder.objType, savedOrder.mousePos, Vector3.zero);
				break;
		}
	}
}
