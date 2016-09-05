using UnityEngine;
using System.Collections;

public class SelectionScript : MonoBehaviour
{
	/*A local script for dragging the selection box & control groups */
	public static bool dragging = false;
	public static Vector3 startPos = Vector3.zero;
	public static Vector3 endPos = Vector3.zero;

	public static Vector3 a = Vector3.zero;
	public static Vector3 b = Vector3.zero;

	public static void HandleSelection(Cl_Orders.MouseOrders mouseOrder)
	{
		//if (Input.GetMouseButtonDown(0))
		if(mouseOrder == Cl_Orders.MouseOrders.SelectionBox_StartDragging)
		{
			dragging = true;
			a =  Vector3.zero;
			b =  Vector3.zero;
			startPos = Input.mousePosition;
		}
		//if (Input.GetMouseButtonUp(0))
		if(mouseOrder == Cl_Orders.MouseOrders.SelectionBox_StopDragging)
		{
			startPos = Vector3.zero;
			endPos = Vector3.zero;
			SelectAllUnitsInTheBox(a, b);
			dragging = false;
		}
		if (dragging)
		{
			endPos = Input.mousePosition;

			//Draw box
			//place selection box
			if (xa.de.selectionBox == null) { return; }
			if (xa.de.hudCamera == null) { return; }

			//Get mouse pos in the world
			Ray ray;
			ray = xa.de.hudCamera.ScreenPointToRay(startPos);
			a = ray.GetPoint(3);
			ray = xa.de.hudCamera.ScreenPointToRay(endPos);
			b = ray.GetPoint(3);
			Debug.DrawLine(a, b, Color.green);

			//Debug.Log("A: " + a + ", B: " + b);
			//Draw lines
			Debug.DrawLine(new Vector3(a.x, a.y, a.z), new Vector3(b.x, a.y, b.z), Color.green);
			Debug.DrawLine(new Vector3(a.x, a.y, a.z), new Vector3(a.x, b.y, b.z), Color.green);
			Debug.DrawLine(new Vector3(b.x, b.y, a.z), new Vector3(a.x, b.y, b.z), Color.green);
			Debug.DrawLine(new Vector3(b.x, b.y, a.z), new Vector3(b.x, a.y, b.z), Color.green);

			//draw box
			Vector3 center = new Vector3((a.x + b.x) * 0.5f, (a.y + b.y) * 0.5f, (a.z + b.z) * 0.5f);
			xa.de.selectionBox.transform.position = center;
			xa.de.selectionBox.transform.SetScaleX(Setup.Distance(a.x, b.x));
			xa.de.selectionBox.transform.SetScaleY(Setup.Distance(a.y, b.y));
		}
		else
		{
			xa.de.selectionBox.transform.position = new Vector3(-999, -999, -999);
			xa.de.selectionBox.transform.SetScaleX(1);
			xa.de.selectionBox.transform.SetScaleY(1);
		}


	}

	public static void SelectAllUnitsInTheBox(Vector3 a, Vector3 b)
	{
		DeselectAllUnits();
		Ray ray;
		//Flip through all objects, find which units are in the box
		for (int i = 0; i < ObjFuncs.objs.Count; i++)
		{
			if (ObjFuncs.objs[i].isUnit && ObjFuncs.objs[i].ownedByClId == hl.local_uId)
			{
				ObjFuncs.objs[i].selected = false;
				//is it inside this selection box?
				Vector3 c = xa.de.globalMainCam.WorldToScreenPoint(ObjFuncs.objs[i].go.transform.position);

				ray = xa.de.hudCamera.ScreenPointToRay(c);
				Vector3 c2 = ray.GetPoint(3);

				if (((c2.x > a.x && c2.x < b.x) || (c2.x < a.x && c2.x > b.x))
					&& ((c2.y > a.y && c2.y < b.y) || (c2.y < a.y && c2.y > b.y))
					)
				{
					ObjFuncs.objs[i].selected = true;
					Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(c2.x, c2.y, c2.z), Color.cyan);
				}
				else
				{
					Debug.DrawLine(new Vector3(0, 0, 0), new Vector3(c2.x, c2.y, c2.z), Color.red);
				}
			}
		}
	}

	public static void DeselectAllUnits()
	{
		//Flip through all objects, find which units are in the box
		for (int i = 0; i < ObjFuncs.objs.Count; i++)
		{
			if (ObjFuncs.objs[i].isUnit)
			{
				ObjFuncs.objs[i].selected = false;
			}
		}
	}
}
