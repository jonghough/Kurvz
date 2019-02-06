using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SphereBehaviour : MonoBehaviour
{

	Kurvz.CatmullRomSpline _spline;
	Vector3 oldPos;
	List<Kurvz.SplineVector> vecs;

	// Use this for initialization
	void Start ()
	{
		vecs = new List<Kurvz.SplineVector> ();
		for (int i = 0; i < 10; i++) {

			vecs.Add (new MySplineVector (new Vector3 (Random.Range (-50, 50), Random.Range (-50, 50), Random.Range (-50, 50))));
		}
		_spline = new Kurvz.CatmullRomSpline (vecs, null);
		StartCoroutine (AddVectors ());
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 v = _spline.UpdateCurveAtSpeed (Time.deltaTime, 15f); //Debug.Log("v "+v);

		float s = (v - oldPos).magnitude / Time.deltaTime;
		Debug.Log ("speed is  " + s);
		
		oldPos = v;
		transform.position = v;
	
	}

	IEnumerator AddVectors ()
	{
		while (true) {
			yield return new WaitForSeconds (1.6f);
			_spline.AddVector (new MySplineVector (new Vector3 (Random.Range (-50, 50), Random.Range (-50, 50), Random.Range (-50, 50))));
		}
	}
}

public class MySplineVector : Kurvz.SplineVector
{

	private Vector3 _vector;

	public MySplineVector (Vector3 vector)
	{
		_vector = vector;
	}

	public Vector3 Position ()
	{
		return _vector;
	}
}
