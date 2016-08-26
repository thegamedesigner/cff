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


	//CreateObj order
	[Command]
	public void CmdSendOrder(int order, int clId, Vector3 mousePos, int objType)//Sends an order from the client to the server
	{
		//On the server, record the order. It will enact it as soon as can (based on lockstep eventually, right now it just enacts it at once)
		OrderFuncs.RecordOrder(order, clId, mousePos, objType);
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
		if (isServer)
		{
			ServerSideLoop.UpdateServer();
		}

		if (isClient)
		{
			ClientSideLoop.UpdateClient();

		}
	}
}
