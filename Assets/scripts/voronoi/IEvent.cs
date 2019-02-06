using System;

namespace VoronoiJx
{
	public interface IEvent /*: IComparable<IEvent>*/{
		float X();

		float Y();

		float GetDistanceToLine();
	}
}

