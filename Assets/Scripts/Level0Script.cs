using UnityEngine;
using System.Collections;

public class Level0Script : MonoBehaviour
{
	public static bool beenToLevel0 = false;
    public static string levelBeforeLevel0 = "";
    void Start()
    {
        beenToLevel0 = true;
        Debug.Log("On level0");
		UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

}
