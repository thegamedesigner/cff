using UnityEngine;
using System.Collections;

public class SnapHUD : MonoBehaviour
{
	public Camera cam;
	public HUDItem[] hudItems = new HUDItem[0];

	Vector3 topLeftCorner = Vector3.zero;
	Vector3 center = Vector3.zero;
	Vector3 bottomRightCorner = Vector3.zero;

	[System.Serializable]
	public class HUDItem
	{
		public string type;
		public GameObject go;
		public enum Hor { Left, Center, Right }
		public enum Ver { Top, Center, Bottom }
		public Hor horSnap = Hor.Left;
		public Ver verSnap = Ver.Top;
	}

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		FindCorners();

		//Update all HUDItem pos's.
		for (int i = 0; i < hudItems.Length; i++)
		{
			if(hudItems[i].horSnap == HUDItem.Hor.Left) {hudItems[i].go.transform.SetX(topLeftCorner.x); }
			if(hudItems[i].horSnap == HUDItem.Hor.Center) {hudItems[i].go.transform.SetX(center.x); }
			if(hudItems[i].horSnap == HUDItem.Hor.Right) {hudItems[i].go.transform.SetX(bottomRightCorner.x); }

			if(hudItems[i].verSnap == HUDItem.Ver.Top) {hudItems[i].go.transform.SetY(topLeftCorner.y); }
			if(hudItems[i].verSnap == HUDItem.Ver.Center) {hudItems[i].go.transform.SetY(center.y); }
			if(hudItems[i].verSnap == HUDItem.Ver.Bottom) {hudItems[i].go.transform.SetY(bottomRightCorner.y); }

			//Debug.Log("HERE " + i + ", x: " + hudItems[i].go.transform.position.x + ", y: " + hudItems[i].go.transform.position.y);
		}
	}


	void FindCorners()
	{
		Ray ray = new Ray();

		ray = cam.ScreenPointToRay(new Vector3(0, Screen.height, 0));
		topLeftCorner = ray.GetPoint(1);

		ray = cam.ScreenPointToRay(new Vector3(Screen.width, 0, 0));
		bottomRightCorner = ray.GetPoint(1);

		ray = cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
		center = ray.GetPoint(1);

		//Debug.Log("Corners: " + topLeftCorner + ", " + bottomRightCorner + ", " + center);
	}
}
