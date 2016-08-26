using UnityEngine;
using System.Collections;

public class DestroyAfterTimer : MonoBehaviour
{
	float timeSet;
	public float timeInSeconds = 0;

	void Start()
	{
		timeSet = Time.timeSinceLevelLoad;
	}

	// Update is called once per frame
	void Update()
	{
		if(Time.timeSinceLevelLoad >= timeInSeconds + timeSet)
		{
			Destroy(this.gameObject);
		}
	}
}
