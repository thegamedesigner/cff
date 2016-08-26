using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class hl : MonoBehaviour
{
	/* This script is for: */
	/*A collection of multiplayer data structures */

	public static HlObj hlObj;
	public static GlobalInfo globalInfo;
	public static List<Peer> peers;
	public static int uIds;//Used in many places. Always unique
	public static string gameName = "DefaultGame";
	public static int maxPlayers = 6;
	public static NetworkManager manager;

	public static int local_uId = -1;//The local uId

	[System.Serializable]
	public class GlobalInfo
	{
		//This is a sendable class. Only basic types of variables
		public string currentLevel;
		public int currentPlayers = 0;
	}

	[System.Serializable]
	public class Peer
	{
		//This is a sendable class. Only basic types of variables
		public string myName = "DefaultPlayer";
		public int uId = -1;
		public string key = "";

	}

}
