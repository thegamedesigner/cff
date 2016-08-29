using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class HlObj : NetworkBehaviour
{
	/* This script is for: */
	/* All RPC/Command functions, inside a NetworkBehaviour */



	/* Command is called on the client & happens on the server, ClientRPC is called on the server & happens on all clients */

	//Chat
	[Command]
	public void CmdChat(string str) { RpcChat(str); if (isServer && !isClient) { ChatScript.ChatLocallyAndLog(str); } }

	[ClientRpc]
	public void RpcChat(string str) { ChatScript.ChatLocallyAndLog(str); }
	
	//Ticks & gamestate
	[ClientRpc]
	public void RpcBroadcastGameState(Ticks.GameState gameState)
	{
		Ticks.ApplyTickOnClient(gameState);

		//Now, on each client, store the gamestate
		Ticks.gameStates.Add(gameState);

		//then apply the gamestate to those objects that have changed
	}

	//Create Obj
	[Command]
	public void CmdCreateObj(int clId, Vector3 pos, int objType)//Sends an order from the client to the server
	{
		int uId = ObjFuncs.GetUniqueId();
		ChatScript.ChatLocally("CreateObj on Sv. uId: " + uId);
		RpcCreateObj(clId, pos, objType, uId);
	}
	[ClientRpc]
	public void RpcCreateObj(int clId, Vector3 pos, int objType, int uId)
	{
		//create the object on all clients
		ChatScript.ChatLocally("CreateObj on Cl. uId: " + uId);
		//Trigger local effect
		Effects.CircleBlip(pos, 5, 3);

		//Create the correct object of type
		ObjFuncs.CreateObj((ObjFuncs.Type)objType, pos, Vector3.zero, clId, uId);
	}

	//Move orders
	[Command]
	public void CmdMoveOrder(int clId, Vector3 pos, int[] unitIds)//Sends an order from the client to the server
	{
		Orders.MoveOrder(pos, clId, unitIds);

		RpcMoveOrder(clId, pos, unitIds);
	}

	[ClientRpc]
	public void RpcMoveOrder(int clId, Vector3 pos, int[] unitIds)
	{
		Orders.MoveOrder(pos, clId, unitIds);

		//Trigger local effect
		Effects.MoveOrderEffect(pos);
	}


	//Join as Peer
	[Command]
	public void CmdRequestJoinAsPeer(string myName)
	{
		//A client has asked to become a new peer.
		hl.Peer peer = new hl.Peer();
		hl.uIds++;
		peer.uId = hl.uIds;
		peer.myName = myName;
		RpcGrantJoinAsPeer(peer);

		//Also update everyone's global info
		hl.globalInfo.currentPlayers++;
		RpcBroadcastGlobalInfo(hl.globalInfo);
	}

	[ClientRpc]
	public void RpcGrantJoinAsPeer(hl.Peer peer)
	{
		if (isLocalPlayer)
		{
			if(hl.peers == null) {hl.peers = new List<hl.Peer>(); }
			hl.peers.Add(peer);
			hl.local_uId = peer.uId;
		}
	}

	//Update global info
	[ClientRpc]
	public void RpcBroadcastGlobalInfo(hl.GlobalInfo g)
	{
		hl.globalInfo = g;
	}

	/* Local functions.
	 * The Awake/Start/Update that run on the script that is on the gameobject that has the Network Identity component on it.
	 */

	void Awake()
	{
		Debug.Log("hlObj exists!");
		DontDestroyOnLoad(this.gameObject);
	}

	void Start()
	{

		if (isServer)
		{
			//Setup server info
			hl.globalInfo = new hl.GlobalInfo();
			hl.globalInfo.currentLevel = "Arena1";
			hl.peers = new List<hl.Peer>();
		}

		if (isLocalPlayer)
		{
			hl.hlObj = this;
			CmdRequestJoinAsPeer("DefaultPlayer");
		}
	}

	void Update()
	{
		if(isLocalPlayer)
		{
			if(Input.GetKeyDown(KeyCode.G)) {ChatScript.ChatLocally("G key pressed! " + hl.local_uId); }
			
			Cl_Orders.InputOrders();//detect keyboard & mouse input, as orders, and send them to the server

		}

		if (isServer)
		{
			ServerSideLoop.UpdateServer();

			Ticks.HandleTicks();//Send out ticks
		}

		if (isClient)
		{
			ClientSideLoop.UpdateClient();
		}
	}
}
