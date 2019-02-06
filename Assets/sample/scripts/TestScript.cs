using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoronoiJx;

public class TestScript : MonoBehaviour
{

	List<VectorNode> vectorNodes = new List<VectorNode> ();
	List<GameObject> gobs = new List<GameObject>();
	public int max;
	VoronoiGenerator vg;
	// Use this for initialization
	void Start ()
	{
		vectorNodes.Add (new VectorNode (12, 58));
		vectorNodes.Add (new VectorNode (46,55));
		for (int i = 0; i < max-2; i++) {
			vectorNodes.Add (new VectorNode (Random.Range (-45.5f, 45.5f), Random.Range (-45.5f, 45.5f)));
		
		}

		foreach (var vb in vectorNodes) {
			GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);
			go.transform.position = new Vector3 (vb.x, vb.y, 0);
			gobs.Add(go);
		}
		vg = new VoronoiGenerator (vectorNodes);
		vg.CreateDiagram ();
		//StartCoroutine(UpdateVoronoi());
			
	}
	private Color color = Color.black;
	private IEnumerator UpdateVoronoi(){
		while(true){
		yield return new WaitForSeconds(0.5f);
		
//		for (int i = 0; i < vg.getNodes ().Count; i++) {
//			VectorNode ce = vg.getNodes () [i];
//			HalfEdge edge = ce.halfEdge;
//
//			HalfEdge f = edge;
//
//		//	Debug.Log ("edge is null?  " + (f == null));
//			while (edge != null) {
//				Vector3 fromV = new Vector3 (edge.Twin ().GetTarget ().X (), edge.Twin ().GetTarget ().Y (), 0);
//				Vector3 toV = new Vector3 (edge.GetTarget ().X (), edge.GetTarget ().Y (), 0);
//				Debug.DrawLine (fromV, toV);
//               
//				edge = edge.Next ();
//				if (edge == null || f == edge)
//					break;
//			}
//
//		}

		for(int i = 0; i < vectorNodes.Count; i++){
			var node = vectorNodes[i];
			Vector3 rand = new Vector3(Random.Range(-10,10), Random.Range(-10,10),0).normalized;
			node.position += rand * 61.5f; 
			gobs[i].transform.position = node.position;
		}

			//vg.Rebuild();
//		vg = new VoronoiGenerator(vectorNodes);
//		vg.createDiagram ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
//color = new Color(Random.Range(0,0.5f),Random.Range(0.1f,0.9f),Random.Range(0f,1f));
		for (int i = 0; i < vg.GetNodes ().Count; i++) {
			VectorNode ce = vg.GetNodes () [i];
			HalfEdge edge = ce.halfEdge;

			HalfEdge f = edge;
			
		//	Debug.Log ("edge is null?  " + (f == null));
			while (edge != null) {
				Vector3 fromV = new Vector3 (edge.Twin ().GetTarget ().X (), edge.Twin ().GetTarget ().Y (), 0);
				Vector3 toV = new Vector3 (edge.GetTarget ().X (), edge.GetTarget ().Y (), 0);
				Debug.DrawLine (fromV, toV, Color.cyan);
               
				edge = edge.Next ();
				if (edge == null || f == edge)
					break;
			}

		}
//
//		for(int i = 0; i < vectorNodes.Count; i++){
//			var node = vectorNodes[i];
//			Vector3 rand = new Vector3(Random.Range(-10,10), Random.Range(-10,10),0).normalized;
//			node.position += rand * 1.5f; 
//			gobs[i].transform.position = node.position;
//		}
//	vg.Rebuild();
	}
}
