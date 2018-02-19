using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowClosestPoint : MonoBehaviour {
	public Vector3 pos;
	void OnDrawGizmos(){
		var col = GetComponent<BoxCollider> ();
		if (!col)
			return;
		Vector3 cp = col.ClosestPoint (pos);

		Gizmos.DrawLine (transform.position, transform.position + col.size * 0.5f);
		Gizmos.DrawSphere (pos, 0.1f);
		Gizmos.DrawWireSphere (cp, 0.1f);
	}
}
