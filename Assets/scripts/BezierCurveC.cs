using System;
using UnityEngine;

namespace Kurvz
{
	/// <summary>
	/// Bezier curve.
	/// </summary>
	public class BezierCurveC
	{
		private SplineVector _p0;
		private SplineVector _p1;
		private SplineVector _p2;
		private SplineVector _p3;
	
		private float _t;
		private float _limit;

		private System.Action _onFinish;

		/// <summary>
		/// Initializes the Cubic Bezier Curve instance. Points0 is start point. Points 1 and 2 are contorl  ponts and point 3 is the 
		/// final point. Time limit is the length of time to traverse the curve.
		/// </summary>
		/// <param name="point0">Point0.</param>
		/// <param name="point1">Point1.</param>
		/// <param name="point2">Point2.</param>
		/// <param name="point3">Point3.</param>
		/// <param name="timeLimit">Time limit.</param>
		public BezierCurveC (SplineVector point0, SplineVector point1, SplineVector point2, SplineVector point3, float timeLimit, System.Action OnFinish = null)
		{
			_p0 = point0;
			_p1 = point1;
			_p2 = point2;
			_p3 = point3;
		
			_limit = timeLimit;
			if (OnFinish != null)
				_onFinish = OnFinish;
		}

		public void ResetFinalPoint (SplineVector v)
		{
			_p3 = v;
		}

		/// <summary>
		/// Updates the bezier path.
		/// </summary>
		/// <returns>The bezier path.</returns>
		/// <param name="deltaTime">Delta time.</param>
		public Vector3 UpdateBezierPath (float deltaTime)
		{
			_t += deltaTime;
			if (_t > _limit) {
				if (_onFinish != null)
					_onFinish ();
				return _p3.Position ();
			}
			float n = Normalize (_t);
			float m = 1 - n;
			return m * m * m * _p0.Position () + m * m * n * 3 * _p1.Position () + m * n * n * 3 * _p2.Position () + n * n * n * _p3.Position ();
		}

		/// <summary>
		/// Updates the bezier path at the given speed (approximately)
		/// </summary>
		/// <returns>The bezier path at speed.</returns>
		/// <param name="deltaTime">Delta time.</param>
		/// <param name="speed">Speed.</param>
		public Vector3 UpdateBezierPathAtSpeed (float deltaTime, float speed)
		{
			// dR/ds = dR/dt * (dt / ds), where ds/dt = magnitude of tangent...
			float realdt = _limit * deltaTime * speed / GetTangentMagnitude ();
			return UpdateBezierPath (realdt);
		}

		/// <summary>
		/// Gets the tangent magnitude.
		/// </summary>
		/// <returns>The tangent magnitude.</returns>
		private float GetTangentMagnitude ()
		{
			if (_t < 0 || _t > _limit)
				return 1;
		
			float n = Normalize (_t);
			float m = 1 - n;
			return  (-3 * m * m * _p0.Position () + 3 * (-2 * m * n + m * m) * _p1.Position () + 3 * (-n * n + 2 * n * m) * _p2.Position () + 3 * n * n * _p3.Position ()).magnitude;
		}


		/// <summary>
		/// Gets the tangent (normalized).
		/// </summary>
		/// <returns>The tangent.</returns>
		public Vector3 GetTangent ()
		{
			if (_t < 0 || _t > _limit)
				return Vector3.zero;
		
			float n = Normalize (_t);
			float m = 1 - n;
			return  (-3 * m * m * _p0.Position () + 3 * (-2 * m * n + m * m) * _p1.Position () + 3 * (-n * n + 2 * n * m) * _p2.Position () + 3 * n * n * _p3.Position ()).normalized;

		}

		public Vector3 Get2ndDerivative ()
		{
			if (_t < 0 || _t > _limit)
				return Vector3.zero;
		
			float n = Normalize (_t);
			float m = 1 - n;
			return  6 * n * (_p3.Position () - _p2.Position ()) - 6 * m * (_p1.Position () - _p0.Position ());
		}

		public Vector3 GetBinormal ()
		{
			return Vector3.Cross (GetTangent (), Get2ndDerivative ());
		}

		/// <summary>
		/// Normalize the specified time.
		/// </summary>
		/// <param name="time">Time.</param>
		private float Normalize (float time)
		{
			return time / _limit;
		}

		public float GetNormalizedTime ()
		{
			return Normalize (_t);
		}

		public float GetTime ()
		{
			return _t / _limit;
		}
	}
}