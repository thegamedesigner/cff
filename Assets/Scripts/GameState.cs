using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour
{
	public List<State> states;

	[System.Serializable]
	public class State //Game State
	{
		
	public static float gameTime = 0;
	public static float gameTimeOffset = 0;
	}
}
