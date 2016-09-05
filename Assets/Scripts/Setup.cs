using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Setup : MonoBehaviour
{
	public static void SetAlpha(GameObject go, float a)
	{
		Color tColour;

		tColour = go.transform.GetComponent<Renderer>().material.color;
		tColour.a = a;
		go.transform.GetComponent<Renderer>().material.color = tColour;
	}
	public static void AddToAlpha(GameObject go, float a)
	{
		Color tColour;

		tColour = go.transform.GetComponent<Renderer>().material.color;
		tColour.a += a;
		go.transform.GetComponent<Renderer>().material.color = tColour;
	}

	public static float Distance(float a, float b)
	{
		return Vector2.Distance(new Vector2(a, 0), new Vector2(b, 0));
	}

	public static float DistBetweenAngles(Vector3 ang1, Vector3 ang2, Vector3 pos)
	{
		float dist = 0;
		Vector3 pos2;
		Vector3 pos3;

		xa.emptyObj.transform.position = pos;
		xa.emptyObj.transform.localEulerAngles = ang1;
		xa.emptyObj.transform.Translate(0, 0, 10);
		pos2 = xa.emptyObj.transform.position;

		xa.emptyObj.transform.position = pos;
		xa.emptyObj.transform.localEulerAngles = ang2;
		xa.emptyObj.transform.Translate(0, 0, 10);
		pos3 = xa.emptyObj.transform.position;


		dist = Vector3.Distance(pos2, pos3);

		//Debug.DrawLine(pos, pos2, Color.green, 3);
		//Debug.DrawLine(pos, pos3, Color.green, 3);
		//Debug.DrawLine(pos2, pos3, Color.green,3);
		//Debug.Log("FUCKINGCALLEDIT: " + dist);

		return (dist);
	}

	public static int RollLeftOrRight(Vector3 pos, Vector3 ang, Vector3 ang3)
	{
		Vector3 pos2;
		Vector3 pos3;
		Vector3 pos4;
		Vector3 ang2;

		xa.emptyObj.transform.position = pos;
		xa.emptyObj.transform.localEulerAngles = ang;
		xa.emptyObj.transform.Translate(50, 0, 0);
		pos2 = xa.emptyObj.transform.position;

		ang2 = ang3;
		xa.emptyObj.transform.position = pos;
		ang2.y -= 15;
		xa.emptyObj.transform.localEulerAngles = ang2;
		xa.emptyObj.transform.Translate(50, 0, 0);
		pos3 = xa.emptyObj.transform.position;

		ang2 = ang3;
		xa.emptyObj.transform.position = pos;
		ang2.y += 15;
		xa.emptyObj.transform.localEulerAngles = ang2;
		xa.emptyObj.transform.Translate(50, 0, 0);
		pos4 = xa.emptyObj.transform.position;

		//Debug.DrawLine(pos, pos3, Color.green);
		//Debug.DrawLine(pos, pos4, Color.green);
		//Debug.DrawLine(pos, pos2, Color.blue);
		//Debug.DrawLine(pos2, pos4, Color.yellow);

		if (Vector3.Distance(pos2, pos3) <= Vector3.Distance(pos2, pos4))
		{
			return (-1);
		}
		else
		{
			return (1);
		}

	}


	public static Vector3 SnapToGrid(Vector3 vec1)
	{
		Vector3 vec2;
		vec2.x = Mathf.Round(vec1.x);
		vec2.y = Mathf.Round(vec1.y);
		vec2.z = Mathf.Round(vec1.z);
		return vec2;
	}

	public static Quaternion SlowTurn(Vector3 yourPos, Quaternion yourRotation, Vector3 targetPos, float speed)
	{
		return SlowTurn(yourPos, yourRotation, targetPos, speed, new Vector3(1, 1, 1));
	}

	public static Quaternion SlowTurn(Vector3 yourPos, Quaternion yourRotation, Vector3 targetPos, float speed, Vector3 flattenAxises)
	{
		//Slow turn code
		xa.emptyObj.transform.position = yourPos;
		xa.emptyObj.transform.LookAt(targetPos);

		if (flattenAxises.x == 0) { xa.emptyObj.transform.SetAngX(yourRotation.eulerAngles.x); }
		if (flattenAxises.y == 0) { xa.emptyObj.transform.SetAngY(yourRotation.eulerAngles.z); }
		if (flattenAxises.z == 0) { xa.emptyObj.transform.SetAngZ(yourRotation.eulerAngles.z); }

		float angle = Quaternion.Angle(yourRotation, xa.emptyObj.transform.rotation);
		if (angle > 0)
		{
			yourRotation = Quaternion.Lerp(yourRotation, xa.emptyObj.transform.rotation, Time.deltaTime * speed / angle);
		}

		return yourRotation;
	}

	public static void GUIDrawX(Vector3 pos, Color color, float size)
	{
		Debug.DrawLine(new Vector3(pos.x - size, pos.y + size, pos.z), new Vector3(pos.x + size, pos.y - size, pos.z), color);
		Debug.DrawLine(new Vector3(pos.x - size, pos.y - size, pos.z), new Vector3(pos.x + size, pos.y + size, pos.z), color);
	}
	public static void GUIDrawX(Vector3 pos, Color color, float size, float time)
	{
		Debug.DrawLine(new Vector3(pos.x - size, pos.y + size, pos.z), new Vector3(pos.x + size, pos.y - size, pos.z), color, time);
		Debug.DrawLine(new Vector3(pos.x - size, pos.y - size, pos.z), new Vector3(pos.x + size, pos.y + size, pos.z), color, time);
	}

	public static void GUIDrawSquare(Vector3 pos, Color color, float size)
	{
		Debug.DrawLine(new Vector3(pos.x - size, pos.y - size, pos.z), new Vector3(pos.x + size, pos.y - size, pos.z), color);
		Debug.DrawLine(new Vector3(pos.x - size, pos.y + size, pos.z), new Vector3(pos.x + size, pos.y + size, pos.z), color);

		Debug.DrawLine(new Vector3(pos.x - size, pos.y + size, pos.z), new Vector3(pos.x - size, pos.y - size, pos.z), color);
		Debug.DrawLine(new Vector3(pos.x + size, pos.y + size, pos.z), new Vector3(pos.x + size, pos.y - size, pos.z), color);
	}

	public static void GUIDrawSquareLocally(Vector3 pos, Color color, float size, Transform transform)
	{
		Vector3 topLeft = transform.TransformPoint(new Vector3(pos.x - size, pos.y + size, pos.z));
		Vector3 topRight = transform.TransformPoint(new Vector3(pos.x + size, pos.y + size, pos.z));
		Vector3 bottomLeft = transform.TransformPoint(new Vector3(pos.x - size, pos.y - size, pos.z));
		Vector3 bottomRight = transform.TransformPoint(new Vector3(pos.x + size, pos.y - size, pos.z));

		Debug.DrawLine(topLeft, topRight, color);
		Debug.DrawLine(bottomLeft, bottomRight, color);
		Debug.DrawLine(topLeft, bottomLeft, color);
		Debug.DrawLine(topRight, bottomRight, color);
	}

	public static void GUIDrawDoubleLine(Vector3 pos1, Vector3 pos2, Color color1, Color color2)
	{
		Debug.DrawLine(new Vector3(pos1.x - 0.1f, pos1.y - 0.1f, pos1.z), new Vector3(pos2.x - 0.1f, pos2.y - 0.1f, pos2.z), color1);
		Debug.DrawLine(new Vector3(pos1.x + 0.1f, pos1.y + 0.1f, pos1.z), new Vector3(pos2.x + 0.1f, pos2.y + 0.1f, pos2.z), color2);

	}

	public static void GUIDrawOffsetLine(Vector3 pos1, Vector3 pos2, Color color1)
	{
		Debug.DrawLine(new Vector3(pos1.x - 0.1f, pos1.y - 0.1f, pos1.z), new Vector3(pos2.x - 0.1f, pos2.y - 0.1f, pos2.z), color1);
	}

	public static Vector3 ProjectVecRotation(Vector3 myPos, Vector3 angAddition, Quaternion myRotation, float dist, Vector3 dir)
	{
		if (!xa.emptyObj) { return (myPos); }//if the object is null, just return the same vector out.
		xa.emptyObj.transform.position = myPos;
		xa.emptyObj.transform.rotation = myRotation;
		xa.emptyObj.transform.localEulerAngles += angAddition;
		xa.emptyObj.transform.Translate(dir * dist);
		return (xa.emptyObj.transform.position);
	}

	public static bool CheckLeftOrRight(Vector3 pos, Quaternion rotation, Vector3 target, Vector3 dir)
	{

		Vector3 v1 = Setup.ProjectVecRotation(pos, new Vector3(0, 0, 90), rotation, 10, dir);
		Vector3 v2 = Setup.ProjectVecRotation(pos, new Vector3(0, 0, -90), rotation, 10, dir);


		if (Vector3.Distance(v1, target) <= Vector3.Distance(v2, target))
		{
			//Setup.drawRope(pos, v1, Color.blue, 0.1f);
			//Setup.drawRope(target, v1, Color.blue, 0.1f);
			//Setup.drawRope(pos, v2, Color.red, 0.1f);
			return (true);
		}
		else
		{
			//Setup.drawRope(pos, v2, Color.blue, 0.1f);
			//Setup.drawRope(target, v2, Color.blue, 0.1f);
			//Setup.drawRope(pos, v1, Color.red, 0.1f);
			return (false);
		}
	}

	public static Quaternion ModifyQuaternion(Quaternion quat, Vector3 vec)
	{
		Vector3 vec1 = quat.eulerAngles;
		vec1 = vec1 + vec;
		return Quaternion.Euler(vec1);
	}

	public static bool isBetweenTheseTwoAngles(Vector3 position, Quaternion rotation, float ang1, float ang2, Vector3 target, Vector3 dir)
	{
		bool check1 = Setup.CheckLeftOrRight(position, ModifyQuaternion(rotation, new Vector3(0, 0, ang1)), target, dir);
		bool check2 = Setup.CheckLeftOrRight(position, ModifyQuaternion(rotation, new Vector3(0, 0, ang2)), target, dir);

		if (check1 && !check2)
		{
			return (true);
		}
		else
		{
			return (false);
		}

	}

	public static string ListOfIntsToString(List<int> ints)
	{
		//Turn this tile into a json
		string str = "{ \"list\" : [";
		bool notTheFirstTime = false;
		for (int i = 0; i < ints.Count; i++)
		{
			if (notTheFirstTime) { str += ", "; }
			else { notTheFirstTime = true; }
			str += ints[i];
		}
		str += "] }";
		return str;
	}

	public static void Log(string str)
	{
		Debug.Log(str);
	}

	public static float XZDistance(Vector3 vec1, Vector3 vec2)
	{
		return Vector3.Distance(new Vector3(vec1.x, 0, vec1.z), new Vector3(vec2.x, 0, vec2.z));
	}

	//true is to the right, false is to the left
	public static bool LeftOrRightOfLine(Vector3 a, Vector3 b, Vector3 q)//A & B are the points the line is drawn between, q is the queried position.
	{
		if (Mathf.Sign((b.x - a.x) * (q.z - a.z) - (b.z - a.z) * (q.x - a.x)) > 0)
		{
			return true;
		}
		else
		{
			return false;
		}

	}

	public static float Dist(float v1, float v2)
	{
		return Vector2.Distance(new Vector2(v1, 0), new Vector2(v2, 0));
	}

	public static float DistXZ(Vector3 v1, Vector3 v2)//A 3d vector dist, that flattens to the xz
	{
		return Vector3.Distance(new Vector3(v1.x, 0, v1.z), new Vector3(v2.x, 0, v2.z));
	}

	public static bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
	{
		Vector3 cp1 = Vector3.Cross(b - a, p1 - a);
		Vector3 cp2 = Vector3.Cross(b - a, p2 - a);
		if (Vector3.Dot(cp1, cp2) >= 0)
		{
			return true;
		}

		return false;
	}

	public static bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
	{
		if (SameSide(p, a, b, c) && SameSide(p, b, a, c) && SameSide(p, c, a, b))
		{
			return true;
		}
		return false;
	}

	public static float Ran(float a, float b)
	{
		float result = Random.Range(a, b);
		return result;
	}

}
