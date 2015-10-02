// Adapted from Sebastian Lague's Unity A* Pathfinding Tutorials
// https://www.youtube.com/playlist?list=PLFt_AvWsXl0cq5Umv3pMC9SPnKjfp9eGW

using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {

	protected Pathfinding pf;

	public Transform player;
	public Transform target;
	public float speed = 20;
	[SerializeField]
	protected Vector3[] path;
	[SerializeField]
	protected int targetIndex;

	[SerializeField]
	protected float distanceToPlayer;
	[Tooltip("The distance between path end and players position before path needs to be recalculated")]
	public float moveDistance;
	[Tooltip("Distance to player before will stop (and take photos etc)")]
	public float maxDistance;

	public bool DrawGizmos;

    

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) {
		if (pathSuccessful) {
			path = newPath;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
		else
		{
            Debug.Log(transform.name);
            Debug.Log(pf.TestIfPosWalkable(transform.position));
            Debug.Log(pf.TestIfPosWalkable(target.transform.position));
            
            Debug.Log (transform.name + " Path Unsuccessful! Start: "+pf.TestIfPosWalkable(transform.position) + "; Target: " + pf.TestIfPosWalkable(target.transform.position));

		}
	}

	virtual protected IEnumerator FollowPath() {
		yield return null;
	}

	public void OnDrawGizmos() {
		if (!DrawGizmos)
			return;

		if (path != null) {
			for (int i = targetIndex; i < path.Length; i ++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one);

				if (i == targetIndex) {
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else {
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}

	protected void RecalculatePath ()
	{
		StopCoroutine("FollowPath");
		PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
	}

	protected bool HasTargetMoved ()
	{
		
		int pos = path.Length - 1;
		if (Vector3.Distance (path[pos], target.position) > moveDistance)
			return true;
		else
			return false;
	}
}
