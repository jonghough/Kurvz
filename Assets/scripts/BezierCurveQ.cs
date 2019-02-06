using System;
using UnityEngine;

namespace Kurvz
{
	/// <summary>
	/// Bezier curve.
	/// </summary>
	public class BezierCurveQ
	{
		private SplineVector _p0;
		private SplineVector _p1;
		private SplineVector _p2;

		private float _t;
		private float _limit;

	
		private System.Action _onFinish;

		/// <summary>
		/// Initializes a new instance of the <see cref="BezierCurve"/> class.
		/// </summary>
		/// <param name="point0">Point0.</param>
		/// <param name="point1">Point1.</param>
		/// <param name="point2">Point2.</param>
		/// <param name="timeLimit">Time limit.</param>
		public BezierCurveQ (SplineVector point0, SplineVector point1, SplineVector point2, float timeLimit, Action onFinish = null)
		{
			_p0 = point0;
			_p1 = point1;
			_p2 = point2;
		
			_limit = timeLimit;
			_onFinish = onFinish;
		}

		public void ResetFinalPoint (SplineVector v)
		{
			_p2 = v;
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
				return _p2.Position ();
			}

			float n = Normalize (_t);
			float m = 1 - n;
			return m * (m * _p0.Position () + n * _p1.Position ()) + n * (m * _p1.Position () + n * _p2.Position ());
		}

		/// <summary>
		/// Updates the bezier path at approximately the given speed.
		/// </summary>
		/// <returns>The bezier path at speed.</returns>
		/// <param name="deltaTime">Delta time.</param>
		/// <param name="speed">Speed.</param>
		public Vector3 UpdateBezierPathAtSpeed (float deltaTime, float speed)
		{
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
			return (-2 * m * _p0.Position () + 2.0f * (m - n) * _p1.Position () + 2 * n * _p2.Position ()).magnitude;
		}

		/// <summary>
		/// Gets the (normalized) tangent of the bezier curve at the current point on the curve.
		/// </summary>
		/// <returns>The tangent.</returns>
		public Vector3 GetTangent ()
		{
			if (_t < 0 || _t > _limit)
				return Vector3.zero;

			float n = Normalize (_t);
			float m = 1 - n;
			return  (-2 * m * _p0.Position () + 2.0f * (m - n) * _p1.Position () + 2 * n * _p2.Position ()).normalized;
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
	}
}