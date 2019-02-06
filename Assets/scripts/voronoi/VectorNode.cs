using System;
using UnityEngine;

namespace VoronoiJx
{
	public class VectorNode
	{
		private Vector2 _vector;
		public float x{get { return _vector.x; }}
		public float y{get { return _vector.y; }}

		private HalfEdge _halfEdge;
		public HalfEdge halfEdge{get{return _halfEdge;} set{_halfEdge = value;}}

		public VectorNode (Vector2 vector)
		{
			_vector = vector;
		}

		public VectorNode(float xv, float yv){
			_vector = new Vector2 (xv, yv);
		}

		public Vector3 position { get { return _vector; } set { _vector = value; }}
	}
}

