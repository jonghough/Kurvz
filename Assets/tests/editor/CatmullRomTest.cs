using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NUnit;
using NUnit.Framework;
using Kurvz;

public class CatmullRomTest {

	[Test]
	public void HighSpeedTest1(){
		List<Kurvz.SplineVector> vecs = new List<Kurvz.SplineVector> ();
		for (int i = 0; i < 10; i++) {

			vecs.Add (new MySplineVector (new Vector3 (Random.Range (-50, 50), Random.Range (-50, 50), Random.Range (-50, 50))));

		}
		// this should be the final point.
		vecs.Add(new MySplineVector (new Vector3 (0,0,0)));
		vecs.Add(new MySplineVector (new Vector3 (40,20,10)));
		var spline = new Kurvz.CatmullRomSpline (vecs, null);

		int timerCount = 0;
		float deltaTime = 1f / 30;
		Vector3 v = new Vector3(1000,1000,1000);
		while(timerCount++ < 30){
			v = spline.UpdateCurveAtSpeed(deltaTime, float.MaxValue);
		}
		
		Assert.That(Vector3.Distance(v,Vector3.zero) < Mathf.Epsilon);
	}

	[Test]
	public void HighSpeedTest2(){
		List<Kurvz.SplineVector> vecs = new List<Kurvz.SplineVector> ();
		for (int i = 0; i < 2; i++) {

			vecs.Add (new MySplineVector (new Vector3 (Random.Range (-50, 50), Random.Range (-50, 50), Random.Range (-50, 50))));

		}
		//
		vecs.Add(new MySplineVector (new Vector3 (0,0,0)));
		vecs.Add(new MySplineVector (new Vector3 (40,20,10)));
		var spline = new Kurvz.CatmullRomSpline (vecs, null);

		int timerCount = 0;
		float deltaTime = 1f / 30;
		Vector3 v = new Vector3(1000,1000,1000);
		while(timerCount++ < 30){
			v = spline.UpdateCurveAtSpeed(deltaTime, float.MaxValue);
		}
		
		Assert.That(Vector3.Distance(v,Vector3.zero) < Mathf.Epsilon);
	}

	[Test]
	public void NotEnoughPointsTest(){
		List<Kurvz.SplineVector> vecs = new List<Kurvz.SplineVector> ();
		
		vecs.Add(new MySplineVector (new Vector3 (0,0,0)));
		vecs.Add(new MySplineVector (new Vector3 (40,20,10)));
		Assert.Throws( typeof(System.Exception), ()=>{
			var spline = new Kurvz.CatmullRomSpline (vecs, null);});

	}

	[Test]
	public void NoPointsTest(){
		Assert.Throws( typeof(System.Exception), ()=>{
			var spline = new Kurvz.CatmullRomSpline (null, null);});
	}

}
