using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Effects : MonoBehaviour
{
	public GameObject moveOrderPrefab;
	public GameObject circleBlipPrefab;
	public GameObject linePrefab;//a 1x1x1 cube, with the renderer turned off
	public GameObject bubblesPrefab;

	public Material red;
	public Material blue;
	public Material green;
	public Material white;
	public Material grey;
	public Material black;
	public Material yellow;
	public Material ltBlue;
	public Material dkBlue;
	public Material cyan;
	public Material orange;

	public enum Colors
	{
		Red,
		Blue,
		Green,
		White,
		Grey,
		Black,
		Yellow,
		LtBlue,
		DkBlue,
		Cyan,
		Orange
	}

	public static void InitEffects()//Must be called before using some of the effects
	{
		InitDrawLines();
	}

	public static void UpdateEffects()
	{
		UpdateLines();
	}

	public static void MoveOrderEffect(Vector3 pos)
	{
		Instantiate(xa.ef.moveOrderPrefab, pos, new Quaternion(0, 0, 0, 0));
	}

	public static void CircleBlip(Vector3 pos) { CircleBlip(pos, 0.3f, 2, 1); }
	public static void CircleBlip(Vector3 pos, float totalTime, float scaleTo, float startingScale)//Creates a expanding & fading disk. Lasts 0.3 seconds before scaing out
	{
		GameObject go = (GameObject)Instantiate(xa.ef.circleBlipPrefab, pos, new Quaternion(0, 0, 0, 0));

		go.transform.SetScaleX(startingScale);
		go.transform.SetScaleZ(startingScale);
		iTween.ScaleTo(go, iTween.Hash("x", scaleTo, "z", scaleTo, "islocal", true, "easetype", iTween.EaseType.easeInOutSine, "time", totalTime));
		iTween.FadeTo(go, iTween.Hash("alpha", 0, "easetype", iTween.EaseType.easeInOutSine, "time", totalTime));

		DestroyAfterTimer script = go.AddComponent<DestroyAfterTimer>();
		script.timeInSeconds = totalTime + 0.1f;
	}

	public static void Bubbles(Vector3 pos)
	{
		GameObject go = (GameObject)Instantiate(xa.ef.bubblesPrefab, pos, new Quaternion(0, 0, 0, 0));
		DestroyAfterTimer script = go.AddComponent<DestroyAfterTimer>();
		script.timeInSeconds = 2f;
	}

	public static List<RequestedLine> requestedLines;
	public static List<Line> lines;

	public class RequestedLine
	{
		public Vector3 a;
		public Vector3 b;
		public float t;
		public Colors c;
	}

	public class Line
	{
		public GameObject go;
		public Renderer rn;
		public bool used = false;
	}

	public static void DrawLine(Vector3 a, Vector3 b, float thickness, Colors color)
	{
		RequestedLine rl = new RequestedLine();
		rl.a = a;
		rl.b = b;
		rl.t = thickness;
		rl.c = color;
		requestedLines.Add(rl);
	}
	public static void UpdateLines()
	{
		//reset the .used for all lines
		for (int i = 0; i < lines.Count; i++) { lines[i].rn.enabled = false; lines[i].used = false; }

		for (int c = 0; c < requestedLines.Count; c++)
		{
			//try to find an active line to use
			for (int i = 0; i < lines.Count; i++)
			{
				if (!lines[i].used)
				{
					lines[i].used = true;
					lines[i].rn.enabled = true;
					lines[i].rn.material = GetMatForColor(requestedLines[c].c);
					lines[i].go.transform.position = requestedLines[c].a;
					lines[i].go.transform.LookAt(requestedLines[c].b);
					float dist = Vector3.Distance(requestedLines[c].a, requestedLines[c].b) * 0.5f;
					lines[i].go.transform.Translate(new Vector3(0, 0, dist));
					lines[i].go.transform.SetScaleX(requestedLines[c].t);
					lines[i].go.transform.SetScaleY(requestedLines[c].t);
					lines[i].go.transform.SetScaleZ(dist * 2);
					break;
				}
			}
		}
		requestedLines = new List<RequestedLine>();
	}

	public static Material GetMatForColor(Colors color)
	{
		switch (color)
		{
			case Colors.Red: return xa.ef.red;
			case Colors.Green: return xa.ef.green;
			case Colors.Black: return xa.ef.black;
			case Colors.White: return xa.ef.white;
			case Colors.Grey: return xa.ef.grey;
			case Colors.Yellow: return xa.ef.yellow;
			case Colors.Orange: return xa.ef.orange;
			case Colors.Blue: return xa.ef.blue;
			case Colors.LtBlue: return xa.ef.ltBlue;
			case Colors.DkBlue: return xa.ef.dkBlue;
			case Colors.Cyan: return xa.ef.cyan;
		}
		return xa.ef.red;
	}

	public static void InitDrawLines()
	{
		requestedLines = new List<RequestedLine>();
		lines = new List<Line>();
		for (int i = 0; i < 100; i++)
		{
			Line line = new Line();
			line.go = (GameObject)Instantiate(xa.ef.linePrefab);
			DontDestroyOnLoad(line.go);
			line.rn = line.go.GetComponent<Renderer>();
			lines.Add(line);
		}
	}
}
