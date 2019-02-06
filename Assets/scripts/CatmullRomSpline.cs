using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kurvz
{
	public class CatmullRomSpline
	{

		/// <summary>
		/// The control points. Curve will pass through these points.
		/// </summary>
		private List<SplineVector> _controlPts;
		/// <summary>
		/// The cached index of the current control point.
		/// </summary>
		//private int _cachedIndex = 0;
		/// <summary>
		/// The callback for finishing the spline.
		/// </summary>
		private System.Action _onFinish;

		/// <summary>
		/// The normalized timer.
		/// </summary>
		private float _normalizedTimer;

		/// <summary>
		/// Flag for reaching end of curve.
		/// </summary>
		private bool _didReachEnd = false;

		private SplineVector _p0, _p1, _p2, _p3;


		/// <summary>
		/// Initializes a new instance of the <see cref="CatmullRomSpline"/> class.
		/// </summary>
		/// <param name="controlPoints">Control points.</param>
		/// <param name="timeLimit">Time limit.</param>
		public CatmullRomSpline (List<SplineVector> controlPoints, System.Action onFinish = null)
		{
			if (controlPoints == null || controlPoints.Count < 4) {
				throw new System.Exception ("Null or not enough control points.");
			}
			_controlPts = new List<SplineVector> (controlPoints);
			_onFinish = onFinish;

			_p0 = _controlPts [0];
			_p1 = _controlPts [1];
			_p2 = _controlPts [2];
			_p3 = _controlPts [3];
		}

		public void AddVector (SplineVector vector)
		{
			_controlPts.Add (vector);
		}

		/// <summary>
		/// Updates the curve.
		/// </summary>
		/// <returns>The curve.</returns>
		/// <param name="deltaTime">Delta time.</param>
		private Vector3 UpdateCurve (float deltaTime)
		{
		
			if (_didReachEnd)
				return _controlPts [1].Position ();

			_normalizedTimer += deltaTime;
			if (_normalizedTimer > 1.0f) {
				_normalizedTimer = 0f;
				_controlPts.RemoveAt (0);
				if (_controlPts.Count < 4) {
					_didReachEnd = true;
					if (_onFinish != null)
						_onFinish ();
					return _controlPts [1].Position ();  
				} else {
					_p0 = _controlPts [0];
					_p1 = _controlPts [1];
					_p2 = _controlPts [2];
					_p3 = _controlPts [3];
				}
			}
			float time = _normalizedTimer;
			// catmull rom calculation.
			return 0.5f * ((2 * _p1.Position ()) +
			(_p2.Position () - _p0.Position ()) * time +
			(2 * _p0.Position () - 5 * _p1.Position () + 4 * _p2.Position () - _p3.Position ()) * time * time +
			(3 * _p1.Position () - _p0.Position () - 3 * _p2.Position () + _p3.Position ()) * time * time * time);    
		}


		/// <summary>
		/// Checks if the time variable has reached the end of the curve. If this is true, the
		/// curve position will not be updated anymore.
		/// </summary>
		/// <returns><c>true</c>, if reach end of curve, <c>false</c> otherwise.</returns>
		public bool DidReachEndOfCurve ()
		{
			return _didReachEnd;
		}


		/// <summary>
		/// Updates the curve at the given speed.THis will move along the curve with the given speed.
		/// </summary>
		/// <returns>The curve at speed.</returns>
		/// <param name="deltaTime">Delta time.</param>
		/// <param name="speed">Speed.</param>
		public Vector3 UpdateCurveAtSpeed (float deltaTime, float speed)
		{
			//this part is a fudge.
			speed = 2 * speed;
			//get the velocity magnitude
			float spd = GetTangentAtCurrentTime ().magnitude;
			//NaN occurs on the last section of the final segment of the curve, occassionally.  Tangent is zero.
			//set the time to the time limit. If NaN, we cannot do further calculations, so end the curve.
			if (float.IsNaN (spd) || float.IsInfinity (spd)) {
				return _controlPts [_controlPts.Count - 2].Position (); 
			}
			if (spd < 0.00001f)
				spd = 0.00001f;
			// create a new deltaTime parameter.
			float realDT = deltaTime * (speed / spd); 
			// update with new parameter.
			return UpdateCurve (realDT);
		}



		/// <summary>
		/// Gets the tangent of the curve at the current time.
		/// </summary>
		/// <returns>The tangent at current time.</returns>
		public Vector3 GetTangentAtCurrentTime ()
		{
			if (_controlPts.Count < 4)
				return Vector3.zero;
			else {
				SplineVector p0 = _controlPts [0];
				SplineVector p1 = _controlPts [1];
				SplineVector p2 = _controlPts [2];
				SplineVector p3 = _controlPts [3]; 
				return (p2.Position () - p0.Position ()) +
				2.0f * (2 * p0.Position () - 5 * p1.Position () + 4 * p2.Position () - p3.Position ()) * _normalizedTimer +
				3.0f * (3 * p1.Position () - p0.Position () - 3 * p2.Position () + p3.Position ()) * _normalizedTimer * _normalizedTimer;
			}

		}

		/// <summary>
		/// Get the 2nd derivative at current time. (i.e. d2R/dt2)
		/// </summary>
		/// <returns>The derivative at current time.</returns>
		public Vector3 Get2ndDerivativeAtCurrentTime ()
		{
			if (_controlPts.Count < 4)
				return Vector3.zero;
			else {
				SplineVector p0 = _controlPts [0];
				SplineVector p1 = _controlPts [0];
				SplineVector p2 = _controlPts [0];
				SplineVector p3 = _controlPts [0]; 
				return 2.0f * (2 * p0.Position () - 5 * p1.Position () + 4 * p2.Position () - p3.Position ()) +
				6.0f * (3 * p1.Position () - p0.Position () - 3 * p2.Position () + p3.Position ()) * _normalizedTimer;
			}
		}

		/// <summary>
		/// Gets the curvature. This is a measure of the 2nd derivative.
		/// </summary>
		/// <returns>The curvature.</returns>
		public float GetCurvature ()
		{
			Vector3 t1 = GetTangentAtCurrentTime ();
			Vector3 t2 = Get2ndDerivativeAtCurrentTime ();
			if (t1.magnitude < 0.001f || t2.magnitude < 0.001f)
				return 0;

			Vector3 cross = Vector3.Cross (t1, t2);
		
			float tang = t1.magnitude;

			if (float.IsNaN (tang) || float.IsInfinity (tang))
				return 0;

			return cross.magnitude / (tang * tang * tang);
		}

		/// <summary>
		/// Gets the binormal (not normalized)
		/// </summary>
		/// <returns>The binormal.</returns>
		public Vector3 GetBinormal ()
		{
			return Vector3.Cross (GetTangentAtCurrentTime (), Get2ndDerivativeAtCurrentTime ());
		}

		/// <summary>
		/// Gets the normal (normalized) (i.e. Binormal x Tangent)
		/// </summary>
		/// <returns>The normal.</returns>
		public Vector3 GetNormal ()
		{
			return Vector3.Cross (GetBinormal (), GetTangentAtCurrentTime ().normalized).normalized;
		}
	}
}