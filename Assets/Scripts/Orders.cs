using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Orders : MonoBehaviour
{
	/*A collection of orders called on the server */
	public static void MoveOrder(Vector3 pos, int clId, int[] ids)//called either on the sv or from the sv
	{
		ObjFuncs.Obj o;
		for (int i = 0; i < ids.Length; i++)
		{
			for (int a = 0; a < ObjFuncs.objs.Count; a++)
			{
				o = ObjFuncs.objs[a];
				if (o.uId == ids[i])
				{
					if (o.isUnit && o.ownedByClId == clId)//double check this is a unit & that it can be ordered by this client
					{
						//tell it to move
						o.goal = pos;
					}
				}
			}
		}
	}
}
