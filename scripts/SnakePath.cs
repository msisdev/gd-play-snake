using Godot;
using System.Collections.Generic;

public class SnakePath
{
	private List<Vector2>   _points = new List<Vector2>();
	public Vector2          this[int index] => _points[index];
	public int              Count => _points.Count;
	private readonly float  _segDist;
  private Vector2         _headPoint => _points[0];

  // Get the last point
  public Vector2          Last => _points.Count > 0 ? _points[^1] : Vector2.Zero;

	public SnakePath(float segDist, Vector2 initialPos)
	{
		_segDist = segDist;
		AddInitialPoint(initialPos);
	}

	public void AddInitialPoint(Vector2 pos)
	{
		_points.Clear();
		_points.Add(pos);
	}

	public void UpdatePath(Vector2 headNextAt, int bodyCount)
	{
    if (_points.Count == 0)
    {
      // This is a first point.
      _points.Add(headNextAt);
      return;
    }

    float travelDist = headNextAt.DistanceTo(_headPoint);

    // If the head moves fast, we should add multiple points.
    // Iterate for the # of new points to add.
    for (int i = 0; i < travelDist / _segDist; i++)
    {
      // Determine the new point by moving from the head point
      Vector2 newPoint = _headPoint + (_headPoint.DirectionTo(headNextAt) * _segDist);

      // Add the new point as head point
      _points.Insert(0, newPoint);
    }

    // Trim unnecessary points.
    while (_points.Count > bodyCount)
    {
      _points.RemoveAt(_points.Count - 1);
    }
	}
}
