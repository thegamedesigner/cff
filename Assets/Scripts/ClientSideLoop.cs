using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClientSideLoop : MonoBehaviour
{
	public static void UpdateClient()
	{
		ManageGame();//Handle game logic, what little needs to happen on the client-side
		
		ObjFuncs.UpdateObjsLocally();//Handles objs locally.
	}

	public static void ManageGame()
	{
		//Switch to the correct level
		if (hl.globalInfo != null)//Ah, I have a version of global info. 
		{
			if (hl.globalInfo.currentLevel != null)
			{
				if (hl.globalInfo.currentLevel != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)//Application.loadedLevelName
				{
					//Then I should change level to match
					UnityEngine.SceneManagement.SceneManager.LoadScene(hl.globalInfo.currentLevel);
				}
			}
		}
	}

}
