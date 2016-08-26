using UnityEngine;
using System.Collections;

public class PrefabLibrary : MonoBehaviour
{
	public LibraryItem[] library = new LibraryItem[0];

	[System.Serializable]
	public class LibraryItem
	{
		public ObjFuncs.Type type;
		public GameObject prefab;
	}

	public static GameObject GetPrefabForType(ObjFuncs.Type type)
	{
		if (xa.pr == null) { Debug.Log("GetPrefabForType returned null! 1");return null; }
		for (int i = 0; i < xa.pr.library.Length; i++)
		{
			if(xa.pr.library[i].type == type) {return xa.pr.library[i].prefab; }
		}
		Debug.Log("GetPrefabForType returned null! 2");
		return null;
	}

}
