using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChatScript : MonoBehaviour
{
	public TextMesh[] chatTexts;
	public TextMesh chatInput;
	public static List<string> strs = new List<string>();
	bool enterTextMode = false;
	string str = "";
	float timeSet = 0;
	bool showCursor = false;
	public static bool typing = false;

	void Update()
	{
		int index = 0;
		while (index < chatTexts.Length)
		{
			if ((strs.Count - 1 - index) >= 0)
			{
				chatTexts[index].text = strs[strs.Count - 1 - index];
			}
			else
			{
				chatTexts[index].text = "";
			}

			index++;
		}

		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (enterTextMode == false)
			{
				enterTextMode = true;
				str = "";
				typing = true;
			}
			else
			{
				enterTextMode = false;
				typing = false;
				hl.hlObj.CmdChat(str);
				str = "";
				chatInput.text = str;
			}
		}

		if (enterTextMode)
		{
			foreach (char c in Input.inputString)
			{
				if (c == "\b"[0])
				{
					if (str.Length != 0)
					{
						str = str.Substring(0, str.Length - 1);
					}
				}
				else
				{
					if (c == "\n"[0] || c == "\r"[0])
					{
					}
					else
					{
						str += c;
					}
				}
			}
			chatInput.text = str;

			float delay = 0.3f;
			if (showCursor)
			{
				delay = 0.6f;
			}
			if (Time.timeSinceLevelLoad > (timeSet + delay))
			{
				timeSet = Time.timeSinceLevelLoad;
				showCursor = !showCursor;
			}
			if (showCursor)
			{
				chatInput.text += "|";
			}
		}



	}

	public static void ChatLocallyAndLog(string str)
	{
		Debug.Log(str);
		ChatLocally(str);

	}
	public static void ChatLocally(string str)
	{

		strs.Add(str);
		if (strs.Count > 8)
		{
			strs.RemoveAt(0);
		}
	}

}
