using System;
using System.Collections.Generic;

namespace VoronoiJx
{
	public class FloatComparer : IComparer<FloatPair>
	{
		public int Compare (FloatPair x, FloatPair y)
		{
			int comp = y.y.CompareTo (x.y);
			if (comp == 0) {
				comp = -y.x.CompareTo (x.x);

			}
			return comp;
		}
	}

	public struct FloatPair
	{
		public float x;
		public float y;

		public FloatPair (IEvent ev)
		{
			x = ev.X ();
			y = ev.GetDistanceToLine ();
		}
	}

	public class VoronoiGenerator
	{

		private List<VectorNode> _nodeList;

		SortedList<FloatPair, IEvent> _eventQueue;

		private List<CircleEvent> _allCircleEvents = new List<CircleEvent> ();


		private float _openEdgeLimit = 1000f;


		private EventTree _eventTree = new EventTree ();


		public VoronoiGenerator (List<VectorNode> nodeList)
		{
			_nodeList = nodeList;
			_eventQueue = new SortedList<FloatPair,IEvent> (new FloatComparer ());

			//Definitely do this. Duplicate points are a big no-no.
			RemoveDuplicatePoints ();
			foreach (var node in _nodeList) {
				var se = new SiteEvent (node);
				_eventQueue.Add (new FloatPair (se), new SiteEvent (node));
			}



		}

		public void Rebuild ()
		{
			_eventQueue = null;
			_eventTree = new EventTree ();
			_allCircleEvents.Clear ();
			_eventQueue = new SortedList<FloatPair,IEvent> (new FloatComparer ());

			//Definitely do this. Duplicate points are a big no-no.
			RemoveDuplicatePoints ();
			foreach (var node in _nodeList) {
				node.halfEdge = null;
				var se = new SiteEvent (node);
				_eventQueue.Add (new FloatPair (se), new SiteEvent (node));
			}
			CreateDiagram ();
		}

		public List<VectorNode> GetNodes ()
		{
			return _nodeList;
		}

		public List<CircleEvent> GetAllCircleEvents ()
		{
			return _allCircleEvents;
		}

		public void SetOpenEdgeLength (float length)
		{
			_openEdgeLimit = Math.Abs (length);
		}



		public void CreateDiagram ()
		{
			while (_eventQueue.Count != 0) {
				int index = _eventQueue.IndexOfKey (_eventQueue.Keys [0]);
				IEvent nextEvent = _eventQueue.Values [index];
				_eventQueue.RemoveAt (0);
				//create the initial node
				if (_eventTree.GetRoot () == null) {
					_eventTree.SetRoot (new EventTree.LeafNode (new TreeItem (((SiteEvent)nextEvent).V ())));
					_eventTree.setMaxY (((SiteEvent)nextEvent).Y ());

				} else {
					if (nextEvent is SiteEvent) {
						HandleSiteEvent ((SiteEvent)nextEvent);
					} else {

						HandleCircleEvent ((CircleEvent)nextEvent);
						_allCircleEvents.Add ((CircleEvent)nextEvent);
					}
				}

				if (_eventTree.GetRoot () != null) {

				}
			}

			//remaining breakpoints...
			FinishUp ();
		}


		private void HandleSiteEvent (SiteEvent siteEvent)
		{

			EventTree.TreeNode node = _eventTree.GetClosest (this, _eventTree.GetRoot (), siteEvent);


			if (node == null || node is EventTree.BreakpointNode) {
				return;
			}
			EventTree.LeafNode closest = (EventTree.LeafNode)node;

			if (closest.GetDisappearEvent () != null) {
				// TODO 
				int index = _eventQueue.IndexOfValue (closest.GetDisappearEvent ());
				_eventQueue.RemoveAt (index);
				//	_priorityQueue.(closest.getDisappearEvent());
				closest.SetDisappearEvent (null);
			}

			List<CircleEvent> circleEvents2 = _eventTree.InsertNewSiteEvent (closest, new TreeItem (siteEvent.V ()));
			foreach (var ce in circleEvents2) {
				_eventQueue.Add (new FloatPair (ce), ce);
			}
		}


		private void HandleCircleEvent (CircleEvent circleEvent)
		{

			HalfEdge CL = circleEvent.L ().GetHalfEdge ();

			if (CL.GetFace () == circleEvent.L ().GetNode ()) {
				CL = CL.Twin ();
			}

			HalfEdge CR = circleEvent.C ().GetHalfEdge ();
			if (CR.GetFace () == circleEvent.R ().GetNode ()) {
				CR = CR.Twin ();

			}

			HalfEdge RC = CR.Twin ();
			RC.SetTarget (circleEvent);
			CL.SetTarget (circleEvent);

			circleEvent.halfEdge = CR;


			EventTree.LeafNode prev = (EventTree.LeafNode)_eventTree.GetPreviousLeaf (circleEvent.GetCenterLeafNode ());
			EventTree.LeafNode next = (EventTree.LeafNode)_eventTree.GetNextLeaf (circleEvent.GetCenterLeafNode ());

			if (prev != null) {
				if (prev.GetDisappearEvent () != null) {
					int index = _eventQueue.IndexOfValue (prev.GetDisappearEvent ());
					_eventQueue.RemoveAt (index);
					prev.SetDisappearEvent (null);
				}
			}

			if (next != null) {
				if (next.GetDisappearEvent () != null) {
					int index = _eventQueue.IndexOfValue (next.GetDisappearEvent ());
					_eventQueue.RemoveAt (index);
					next.SetDisappearEvent (null);
				}
			}

			List<CircleEvent> newCircles = _eventTree.RemoveNode (circleEvent, prev, circleEvent.GetCenterLeafNode (), next);
			if (newCircles != null) {
				foreach (CircleEvent ce in newCircles) {
					_eventQueue.Add (new FloatPair (ce), ce);
				}
			}

		}


		private void RemoveDuplicatePoints ()
		{
			if (_nodeList == null)
				return;

			Dictionary<float, VectorNode> compressor = new Dictionary<float, VectorNode> ();
			foreach (VectorNode n in _nodeList) {
				compressor.Add (11013.7007f * n.x + 3.2f * n.y - 11.3001f * (n.x * n.y), n);
			}
			_nodeList.Clear ();
			_nodeList.AddRange (compressor.Values);
		}

		private void FinishUp ()
		{
			GetFinalNodePoint (_eventTree.GetRoot ());
		}


		private void GetFinalNodePoint (EventTree.TreeNode node)
		{


			if (node is EventTree.LeafNode) {
				if (((EventTree.LeafNode)node).GetBreakpointNode () == null)
					return;
				Breakpoint b = ((EventTree.LeafNode)node).GetBreakpointNode ().GetBreakpoint ();


				VectorNode n1 = b.getLeftListEvent ().GetHalfEdge ().GetFace ();
				VectorNode n2 = b.getLeftListEvent ().GetHalfEdge ().Twin ().GetFace ();


				float centerx = 0.5f * (b.getLeftListEvent ().GetHalfEdge ().GetFace ().x + b.getLeftListEvent ().GetHalfEdge ().Twin ().GetFace ().x);
				float centery = 0.5f * (b.getLeftListEvent ().GetHalfEdge ().GetFace ().y + b.getLeftListEvent ().GetHalfEdge ().Twin ().GetFace ().y);

				if (n1.y == n2.y) {
					HalfEdge he = b.getLeftListEvent ().GetHalfEdge ();
					CircleEvent ce = new CircleEvent (centerx, -_openEdgeLimit /* neg infinity */);
					if (he.GetTarget () == null) {
						he.SetTarget (ce);
					} else
						he.Twin ().SetTarget (ce);

					_allCircleEvents.Add (ce);
				} else {

					float grad = (n2.y - n1.y) * 1.0f / (n2.x - n1.x);
					float realGrad = -1.0f / grad;

					float constant = centery - realGrad * centerx;

					float bpx = b.getX () - centerx;
					float bpy = b.getY () - centery;

					//if x = bpx...
					float testx = centerx + 10000f;
					float testy = testx * realGrad + constant;
					CircleEvent ce;
					if (testx * bpx + testy * bpy > 0)
						ce = new CircleEvent (testx, testy);
					else
						ce = new CircleEvent (centerx - 10000, (centerx - 10000) * realGrad + constant);


					HalfEdge he = b.getLeftListEvent ().GetHalfEdge ();
					if (he.GetFace () != b.getLeftListEvent ().GetNode ()) {
						he = he.Twin ();
					}


					if (he.GetTarget () == null) {
						he.SetTarget (ce);
					} else if (he.Twin ().GetTarget () == null) {
						he.Twin ().SetTarget (ce);
					} else {
						//big problem... should never happen
					}
					_allCircleEvents.Add (ce);
				}

				return;
			} else {
				Breakpoint b = ((EventTree.BreakpointNode)node).GetBreakpoint ();

				b.CalculateBreakpoint (_openEdgeLimit);


				if (node.LChild () != null) {
					GetFinalNodePoint (node.LChild ());

				}
				if (node.RChild () != null) {
					GetFinalNodePoint (node.RChild ());
				}
			}
		}
	}
}

