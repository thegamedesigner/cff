using UnityEngine;
using System.Collections;

public class Effects : MonoBehaviour
{
	public GameObject moveOrderPrefab;
	public GameObject circleBlipPrefab;
	
	public static void MoveOrderEffect(Vector3 pos)
	{
		Instantiate(xa.ef.moveOrderPrefab,pos,new Quaternion(0,0,0,0));
	}

	public static void CircleBlip(Vector3 pos) {CircleBlip(pos,0.3f,1); }
	public static void CircleBlip(Vector3 pos, float totalTime, float scaleTo)//Creates a expanding & fading disk. Lasts 0.3 seconds before scaing out
	{
		GameObject go = (GameObject)Instantiate(xa.ef.circleBlipPrefab,pos,new Quaternion(0,0,0,0));
		
		go.transform.SetScaleX(0);
		go.transform.SetScaleZ(0);
		iTween.ScaleTo(go, iTween.Hash("x", scaleTo, "z", scaleTo, "islocal", true, "easetype", iTween.EaseType.easeInOutSine, "time", totalTime));
		iTween.FadeTo(go, iTween.Hash("alpha", 0, "easetype", iTween.EaseType.easeInOutSine, "time", totalTime));

		DestroyAfterTimer script = go.AddComponent<DestroyAfterTimer>();
		script.timeInSeconds = totalTime + 0.1f;
	}

}
