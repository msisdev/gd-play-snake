using Godot;
using System.Collections.Generic;

public partial class Snake : Node2D
{
  // Snake movement speed
	[Export] public float Speed = 200.0f;
  // Distance between each body segment
	[Export] public float BodyDist = 10.0f;
  // Minimum distance the head must be from the mouse to move towards it
  [Export] public float HeadMouseMoveDistance = 5.0f;
  // Snake segment scene
	private PackedScene _segmentScene = GD.Load<PackedScene>("res://scenes/snake_segment.tscn");
  // List of segment nodes
	private List<Node2D> _body = new List<Node2D>();
  // Reference to the head segment
  private Node2D _head => _body.Count > 0 ? _body[0] : null;
  // Path data manager
	private SnakePath _path;

	public override void _Ready()
	{
		AddSegment(GlobalPosition);
		_path = new SnakePath(BodyDist, _body[0].GlobalPosition);
	}

	public override void _Process(double delta)
	{
		float fDelta = (float)delta;

		if (Input.IsMouseButtonPressed(MouseButton.Left))
		{
			MoveSnake(fDelta);
		}

		if (Input.IsKeyPressed(Key.Space))
		{
			AddSegment(_body[^1].GlobalPosition);
		}
	}

  /// <summary>
  /// To move the snake,
  /// <list type="bullet">Compare distance between head and mouse.</list>
  /// <list type="bullet">If the distance is greater than the threshold, update the head and body position.</list>
  /// </summary>
	private void MoveSnake(float delta)
	{
    Vector2   headAt    = _head.GlobalPosition;
		Vector2   mouseAt   = GetGlobalMousePosition();
		Vector2   dir       = (mouseAt - _head.GlobalPosition).Normalized();

		// Compare head-mouse dist with threshold.
	  if (headAt.DistanceTo(mouseAt) > HeadMouseMoveDistance)
		{
			// With the new head position, add the new points to the path.
			_path.UpdatePath(headAt + dir * Speed * delta, _body.Count);

			// Move body positions.
			MoveSnakeBody();

			// Move head position.
			_head.GlobalPosition = headAt + dir * Speed * delta;
		}
	}

	private void MoveSnakeBody()
	{
		if (_path.Count == 0) return;
		
		// Get dist between: current head position to the new head position
		float headTravelDist = _body[0].GlobalPosition.DistanceTo(_path[0]);

		// 
		float lerpFactor = Mathf.Clamp(headTravelDist / BodyDist, 0f, 1f);

		for (int i = 1; i < _body.Count; i++)
		{
			// Move body[i] if there is a point[i] for it
			if (i < _path.Count)
			{
				Vector2 startPos = _path[i];
				Vector2 endPos = _path[i - 1];

				// 
				_body[i].GlobalPosition = startPos.Lerp(endPos, lerpFactor);
			}
			// Otherwise, the body[i] should be overlapped with the last point.
			else
			{
				_body[i].GlobalPosition = _path.Last;
			}
		}
	}
 
	private void AddSegment(Vector2 spawnPos)
	{
		var newSegment = _segmentScene.Instantiate<Node2D>();
		newSegment.GlobalPosition = spawnPos;

		AddChild(newSegment);
		_body.Add(newSegment);
	}
}
