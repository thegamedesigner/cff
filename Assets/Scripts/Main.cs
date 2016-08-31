using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{
	public static Main self;

	void Awake()
	{
		if (!self)
		{
			self = this;
			DontDestroyOnLoad(this.gameObject);
		}
		else
		{
			Debug.Log("TimeLord already exists! Killing self!");
			Destroy(this.gameObject);//A TimeLord already exists
		}

		xa.de = this.gameObject.GetComponent<Defines>();
		xa.emptyObj = new GameObject("emptyObj");
		DontDestroyOnLoad(xa.emptyObj);
		xa.mainNodeObj = this.gameObject;

	}

	//First function called in the entire game, on level load (except awake)
	void Start()
	{
		xa.ma = (Main)(this.gameObject.GetComponent("Main"));
		xa.de = (Defines)(this.gameObject.GetComponent("Defines"));
		xa.ef = (Effects)(this.gameObject.GetComponent("Effects"));
		xa.pr = (PrefabLibrary)(this.gameObject.GetComponent("PrefabLibrary"));

		Effects.InitEffects();
	}

	void Update()
	{
		Effects.UpdateEffects();

		if (xa.lastGoal.x != xa.goal.x && xa.lastGoal.z != xa.goal.z)
		{
			xa.lastGoal = xa.goal;
			if(xa.de.debugText.text.Length > 1000) {xa.de.debugText.text = ""; }
			xa.de.debugText.text += "\nGoal: " + xa.goal;
		}
	}

	void OnGUI()
	{
	}


}
