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

	public class GameState
	{
		public float timeStamp = -1;//In ms
		public bool applied = false;
		public int[] uIds;
		public int[] syncdVars;
		public object[] newValues;
	}

	public static float tickRate = 0.05f;
	public static float tickRateTimeSet;

	public static void HandleTicks()
	{
		//Send ticks to all clients, based on the tickRate
		if (Time.timeSinceLevelLoad >= (tickRate + tickRateTimeSet))
		{
			tickRateTimeSet = Time.timeSinceLevelLoad;

			GameState g = CreateGameState();

			//send this tick
			hl.hlObj.RpcBroadcastGameState(g);
		}

	}

	public static GameState CreateGameState()
	{
		//Prepare a gameState
		GameState g = new GameState();//gamestate.
		GameState cg = new GameState();//Complete gamestate. Not pared down based on what has changed.
		g.timeStamp = Time.timeSinceLevelLoad;

		//Create some temp lists to fill
		List<int> uIds = new List<int>();
		List<SyncdVars> syncdVars = new List<SyncdVars>();
		List<object> newValues = new List<object>();

		//loop through every object
		for (int i = 0; i < ObjFuncs.objs.Count; i++)
		{
			//do this part
			ObjFuncs.Obj o = ObjFuncs.objs[i];

			//Add all the syncd vars,
			//whether or not they're different from last time
			//(this is moving them into the same format, so I can more easily check if they've changed from last time)

			//Goal
			uIds.Add(o.uId);
			syncdVars.Add(SyncdVars.Goal);
			newValues.Add((object)o.goal);
		}

		//Copy to cg, CompleteGamestate, so I can store that.
		cg.uIds = new int[uIds.Count];
		cg.syncdVars = new int[syncdVars.Count];
		cg.newValues = new object[newValues.Count];
		for (int i = 0; i < cg.uIds.Length; i++) { cg.uIds[i] = uIds[i]; }
		for (int i = 0; i < cg.syncdVars.Length; i++) { cg.syncdVars[i] = (int)syncdVars[i]; }
		for (int i = 0; i < cg.newValues.Length; i++) { cg.newValues[i] = newValues[i]; }
		completeGameStates.Add(cg);

		//Now compare it to the last complete game state, and cut out all the parts that haven't changed values
		//...Do this here...
		//(dont bother right now, just send complete game states until the system is proven to work, then add this)

		//add the remaining items on the list to the gamestate
		g.uIds = new int[uIds.Count];
		g.syncdVars = new int[syncdVars.Count];
		g.newValues = new object[newValues.Count];
		for (int i = 0; i < g.uIds.Length; i++) { g.uIds[i] = uIds[i]; }
		for (int i = 0; i < g.syncdVars.Length; i++) { g.syncdVars[i] = (int)syncdVars[i]; }
		for (int i = 0; i < g.newValues.Length; i++) { g.newValues[i] = newValues[i]; }

		return g;
	}

	//Called on the client, applys these changes to the objects
	public static void ApplyTickOnClient(GameState g)
	{
		//Create some temp lists to fill
		List<int> uIds = new List<int>();
		List<SyncdVars> syncdVars = new List<SyncdVars>();
		List<object> newValues = new List<object>();

		//fill the lists
		for (int i = 0; i < g.uIds.Length; i++) { uIds.Add(uIds[i]); }
		for (int i = 0; i < g.syncdVars.Length; i++) { syncdVars.Add((SyncdVars)syncdVars[i]); }
		for (int i = 0; i < g.newValues.Length; i++) { newValues.Add(newValues[i]); }
		ObjFuncs.Obj o;

		//go through every item in the lists
		for (int i = 0; i < uIds.Count; i++)
		{
			//find the object
			if (uIds[i] < ObjFuncs.objs.Count && uIds[i] >= 0)//Isn't an invalid ID as far as we can tell
			{
				if (ObjFuncs.objs[i] != null)
				{
					o = ObjFuncs.objs[i];

					//now apply to the objs. (doesn't always have to apply directly to the source variable. It could sync from pos to desiredPos, for example, and assume the unit will handle that itself.
					switch (syncdVars[i])
					{
						case SyncdVars.Goal:o.goal = (Vector3)newValues[i];break;
					}
				}
			}
		}

	}


}
