using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ticks : MonoBehaviour
{
	/* A system that checks if certain variables in Objs have changed, and includes them in an update tick if they have 
	 */

	public static List<GameState> gameStates = new List<GameState>();
	public static List<GameState> completeGameStates = new List<GameState>();

	public enum SyncdVars
	{
		None = 0,
		Goal = 1,
		End
	}

	[System.Serializable]
	public class GameState
	{
		public float timeStamp = -1;//In ms
		public bool applied = false;
		public GameItem[] items;
	}

	[System.Serializable]
	public class GameItem
	{
		public int uId;
		public int syncdVar;
		public float[] newValue;
	}

	public static float tickRate = 0.05f;
	public static float tickRateTimeSet;

	public static void HandleTicks()
	{
		//Send ticks to all clients, based on the tickRate
		if (Time.timeSinceLevelLoad >= (tickRate + tickRateTimeSet))
		{
			tickRateTimeSet = Time.timeSinceLevelLoad;

			if (hl.hlObj != null)
			{
				GameState g = CreateGameState();

				if(g != null)
				{
				//send this tick
				hl.hlObj.RpcBroadcastGameState(g);
				}
			}
		}

	}

	public static GameState CreateGameState()
	{
		//Prepare a gameState
		GameState g = new GameState();//gamestate. A GameState is a list of GameItems.
		GameState cg = new GameState();//Complete gamestate. Not pared down based on what has changed.
		g.timeStamp = Time.timeSinceLevelLoad;

		//Create some temp lists to fill
		List<GameItem> items = new List<GameItem>();

		if (ObjFuncs.objs.Count <= 0) { return null; }
		//loop through every object
		for (int i = 0; i < ObjFuncs.objs.Count; i++)
		{
			//do this part
			ObjFuncs.Obj o = ObjFuncs.objs[i];

			//Add all the syncd vars,
			//whether or not they're different from last time
			//(this is moving them into the same format, so I can more easily check if they've changed from last time)

			//Goal
			GameItem item = new GameItem();
			item.uId = o.uId;
			item.syncdVar = (int)SyncdVars.Goal;
			item.newValue = new float[3];
			item.newValue[0] = o.goal.x;
			item.newValue[1] = o.goal.y;
			item.newValue[2] = o.goal.z;

			items.Add(item);
		}

		//Copy to cg, CompleteGamestate, so I can store that.
		cg.items = new GameItem[items.Count];
		for (int i = 0; i < cg.items.Length; i++)
		{
			cg.items[i] = new GameItem();
			cg.items[i].uId = items[i].uId;
			cg.items[i].syncdVar = items[i].syncdVar;
			cg.items[i].newValue = items[i].newValue;

		}
		completeGameStates.Add(cg);

		//Now compare it to the last complete game state, and cut out all the parts that haven't changed values
		//...Do this here...
		//(dont bother right now, just send complete game states until the system is proven to work, then add this)

		//add the remaining items on the list to the gamestate
		g.items = new GameItem[items.Count];
		for (int i = 0; i < g.items.Length; i++)
		{
			g.items[i] = new GameItem();
			g.items[i].uId = items[i].uId;
			g.items[i].syncdVar = items[i].syncdVar;
			g.items[i].newValue = items[i].newValue;

		}

		return g;
	}

	//Called on the client, applys these changes to the objects
	public static void ApplyTickOnClient(GameState g)
	{
		//Create a temp list to fill
		List<GameItem> items = new List<GameItem>();

		//fill the list
		for (int i = 0; i < g.items.Length; i++)
		{
			GameItem gi = new GameItem();
			gi.uId = g.items[i].uId;
			gi.syncdVar = g.items[i].syncdVar;
			gi.newValue = g.items[i].newValue;
			items.Add(gi);
		}
		ObjFuncs.Obj o;

		//go through every item in the lists
		for (int i = 0; i < items.Count; i++)
		{
			//find the object, by looping through all objects and comparing the ID

			int index = ObjFuncs.GetObjIndexForUID(items[i].uId);
			if (index != -1)
			{
				if (ObjFuncs.objs[index] != null)
				{
					o = ObjFuncs.objs[index];

					//now apply to the objs. (doesn't always have to apply directly to the source variable. It could sync from pos to desiredPos, for example, and assume the unit will handle that itself.
					switch ((SyncdVars)items[i].syncdVar)
					{
						case SyncdVars.Goal:
							float[] v = items[i].newValue;
							o.goal = new Vector3((float)v[0], (float)v[1], (float)v[2]);
							xa.goal = o.goal;
							break;

					}
				}
			}
		}

	}


}
