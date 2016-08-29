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
	}

}
