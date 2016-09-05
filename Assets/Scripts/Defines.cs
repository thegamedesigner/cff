using UnityEngine;
using System.Collections;

public class Defines : MonoBehaviour
{
	public LayerMask toHitFloorMask;
	public LayerMask toHitDraggableObjs;
	public GameObject selectionBox;
	public Camera globalMainCam;
	public Camera hudCamera;
	public GameObject selectionCirclePrefab;
	public TextMesh debugText;
	public GameObject gridSqPrefab;
	public Material defaultHullMat;
	public Material validHullMat;

}
