using UnityEngine;
using System.Collections;

public class MathFuncs : MonoBehaviour
{
	/*Project a position out from a position, euler angles & a dist.
	*/
	public static Vector3 ProjectVec(Vector3 vec1, Vector3 ang, float dist, Vector3 dir)
	{
		if (!xa.emptyObj)
		{
			GameObject go = new GameObject();
			xa.emptyObj = go;
		}
		xa.emptyObj.transform.position = vec1;
		xa.emptyObj.transform.localEulerAngles = ang;
		xa.emptyObj.transform.Translate(dir * dist);
		return (xa.emptyObj.transform.position);
	}

	
	/*First vec is the unit's position, the second is the XYZ of what you're pointing at.
	*/
	public static float ReturnAngleTowardsVec(Vector3 vec1, Vector3 vec2, bool flattenToXZ)
	{
		if(flattenToXZ){vec1.y = vec2.y;}

		xa.emptyObj.transform.position = vec1;
		xa.emptyObj.transform.LookAt(vec2);
		//Debug.Log("ReturnAngleTowardsVec returned: " + xa.emptyObj.transform.localEulerAngles.y);
		return (xa.emptyObj.transform.localEulerAngles.y);
	}

	/* Check whether a point is within a cone
	 */
	public static bool CheckCone(float maxDist, float width, float angle, Vector3 coneOrigin, Vector3 pos, bool flattenToXZ)
	{
		//A unity-centric attempt at this function.
		if (Vector2.Distance(coneOrigin, pos) <= maxDist || maxDist == -1)//First, is it within the dist and/or is there no dist?
		{
			float angDirectlyToPos = MathFuncs.ReturnAngleTowardsVec(coneOrigin, pos, flattenToXZ);
			float diff = Mathf.Abs(Mathf.DeltaAngle(angDirectlyToPos, angle));
			
			if (diff <= width) { return true; }
		}
		return false;
	}

}
