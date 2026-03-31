using Godot;
using System.Collections.Generic;

public partial class Snake : Node2D
{
	[Export] public float Speed = 200.0f;
	[Export] public float SegmentDistance = 4.0f;

	private PackedScene _segmentScene = GD.Load<PackedScene>("res://scenes/snake_segment.tscn");

	private List<Node2D> _body = new List<Node2D>();
	
	private SnakePath _path;

	public override void _Ready()
	{
		_path = new SnakePath(SegmentDistance);
		AddSegment(GlobalPosition);
		_path.AddInitialPoint(_body[0].GlobalPosition);
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

	private void MoveSnake(float delta)
	{
		Node2D head = _body[0];
		Vector2 mousePos = GetGlobalMousePosition();
		Vector2 dir = (mousePos - head.GlobalPosition).Normalized();

		if (head.GlobalPosition.DistanceTo(mousePos) > 5)
		{
			head.GlobalPosition += dir * Speed * delta;

			_path.UpdatePath(head.GlobalPosition, _body.Count);

			UpdateBodyPositions();
		}
	}

	private void UpdateBodyPositions()
	{
		if (_path.Count == 0) return;
		
		float distToLastPoint = _body[0].GlobalPosition.DistanceTo(_path[0]);
		float t = Mathf.Clamp(distToLastPoint / SegmentDistance, 0f, 1f);

		for (int i = 1; i < _body.Count; i++)
		{
			if (i < _path.Count)
			{
				Vector2 startPos = _path[i];
				Vector2 endPos = _path[i - 1];
				_body[i].GlobalPosition = startPos.Lerp(endPos, t);
			}
			else
			{
				_body[i].GlobalPosition = _path.GetLastPoint();
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
