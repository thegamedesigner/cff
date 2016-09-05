using UnityEngine;
using System.Collections;

public class Info : MonoBehaviour
{
	//contains a connection to the obj uid
	[HideInInspector]
	public int uId = -1;

	public Renderer[] mainBodyRenderers;
}
