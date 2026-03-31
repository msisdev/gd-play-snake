using Godot;
using System.Collections.Generic;

public class SnakePath
{
	private List<Vector2> _points = new List<Vector2>();
	private readonly float _segmentDistance;

	public int Count => _points.Count;

	public Vector2 this[int index] => _points[index];

	public SnakePath(float segmentDistance)
	{
		_segmentDistance = segmentDistance;
	}

	public void AddInitialPoint(Vector2 pos)
	{
		_points.Clear();
		_points.Add(pos);
	}

	public void UpdatePath(Vector2 headPos, int bodyCount)
	{
		while (_points.Count > 0 && headPos.DistanceTo(_points[0]) >= _segmentDistance)
		{
			Vector2 newPoint = _points[0] + (_points[0].DirectionTo(headPos) * _segmentDistance);
			_points.Insert(0, newPoint);

			// Trim the points list so it doesn't exceed the number of body segments.
			while (_points.Count > bodyCount)
			{
				_points.RemoveAt(_points.Count - 1);
			}
		}
	}

	public Vector2 GetLastPoint()
	{
		return _points.Count > 0 ? _points[^1] : Vector2.Zero;
	}
}
