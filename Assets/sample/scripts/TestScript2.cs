using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VoronoiJx;

public class TestScript2 : MonoBehaviour {

	public GameObject[] movingObjects;
List<VectorNode> vectorNodes = new List<VectorNode> ();
	List<Kurvz.CatmullRomSpline> splines = new List<Kurvz.CatmullRomSpline>();

	VoronoiGenerator vg;
	// Use this for initialization
	void Start () {
		foreach(var gob in movingObjects){
			Vector3 pos = gob.transform.position;
			vectorNodes.Add(new VectorNode(new Vector2(pos.x, pos.x)));
			var vecs = new List<Kurvz.SplineVector> ();
			for(int i = 0; i < 10; i++){
				vecs.Add (new MySplineVector (new Vector3 (Random.Range(-50,50), Random.Range (-50, 50),0)));
			}
			var crSpline = new Kurvz.CatmullRomSpline(vecs);
			splines.Add(crSpline);
		}
		vg = new VoronoiGenerator (vectorNodes);
		vg.CreateDiagram ();
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < movingObjects.Length; i++){
			var obj = movingObjects[i];
			obj.transform.position = splines[i].UpdateCurveAtSpeed(Time.deltaTime, 5);
			vectorNodes[i].position = obj.transform.position;
		
		}


		for (int i = 0; i < vg.GetNodes ().Count; i++) {
			VectorNode ce = vg.GetNodes () [i];
			HalfEdge edge = ce.halfEdge;

			HalfEdge f = edge;
			
			while (edge != null) {
				Vector3 fromV = new Vector3 (edge.Twin ().GetTarget ().X (), edge.Twin ().GetTarget ().Y (), 0);
				Vector3 toV = new Vector3 (edge.GetTarget ().X (), edge.GetTarget ().Y (), 0);
				Debug.DrawLine (fromV, toV, Color.cyan);
               
				edge = edge.Next ();
				if (edge == null || f == edge)
					break;
			}
			
		}
		vg.Rebuild();
	}
}
