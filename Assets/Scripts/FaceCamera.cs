using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{

	}

	void Update()
	{
		if(xa.de == null) {return; }
		if(xa.de.globalMainCam == null) {return; }
		transform.LookAt(xa.de.globalMainCam.transform.position, Vector3.back);

	}
}
