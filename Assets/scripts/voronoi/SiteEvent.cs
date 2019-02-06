using System;
using UnityEngine;

namespace VoronoiJx
{
	public class SiteEvent : IEvent
	{
		private VectorNode _vector;
		public SiteEvent ( VectorNode vector)
		{
			_vector = vector;
		}

		public VectorNode V(){
			return _vector;
		}

		public float X(){
			return _vector.x;
		}

		public float Y(){
			return _vector.y;
		}

		public float GetDistanceToLine(){
			return Y ();
		}
	}
}

