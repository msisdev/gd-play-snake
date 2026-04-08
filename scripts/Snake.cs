using Godot;
using System;
using System.Collections.Generic;

public partial class Snake : Node2D
{
	[Export] private float _spd = 200f;
	[Export] private float _rspd = 5f;
	[Export] private Vector2 _face = Vector2.Up;
	[Export] private PackedScene _segmentScene = ResourceLoader.Load<PackedScene>("res://scenes/snake_segment.tscn");
	[Export] private int _initBodyCount = 10;
	[Export] private float _segmentSpacing = 5f;

	private Node2D _head;
	private List<Node2D> _body = new List<Node2D>();
	private List<Vector2> _path = new List<Vector2>();
	
	public override void _Ready()
	{
		// Randomize the initial face direction.
		_face = Vector2.FromAngle((float)(Random.Shared.NextDouble() * Math.PI * 2));

		// Put head.
		{
			_head = _segmentScene.Instantiate<Node2D>();
			_head.Position = Position;
			AddChild(_head);
			_path.Insert(0, _head.Position);
		}

		// Add body segments.
		for (int i = 0; i < _initBodyCount; i++)
		{
			_Grow();
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsMouseButtonPressed(MouseButton.Left))
		{
			_Move(delta);
		}
		if (Input.IsPhysicalKeyPressed(Key.Space))
		{
			_Grow();
		}
	}

	// Add a new body segment.
	private void _Grow()
	{
		var newSegment = _segmentScene.Instantiate<Node2D>();
		newSegment.Position = _body.Count > 0 ? _body[^1].Position : _head.Position;
		_body.Add(newSegment);
		AddChild(newSegment);
	}

	// Move the snake
	private void _Move(double delta)
	{
		// Rotate the face vector towards the mouse.
		{
			var mousePos = GetGlobalMousePosition();
			var toMouse = (mousePos - _head.GlobalPosition).Normalized();
			_face = _face.Lerp(toMouse, (float)delta * _rspd).Normalized();
		}

		// Move head.
		{
			_head.Position += _face * _spd * (float)delta;
		}

		// Update path.
		{
			if (_head.Position.DistanceTo(_path[0]) >= _segmentSpacing)
			{
				_path.Insert(0, _head.Position);
				while (_path.Count > _body.Count + 1)
				{
					_path.RemoveAt(_path.Count - 1);
				}
			}
		}

		// Move bodies.
		{
			var headToPathLen = _head.Position.DistanceTo(_path[0]);
			var lerpWeight = Math.Min(1f, headToPathLen / _segmentSpacing);
			for (int i = 0; i < _body.Count; i++)
			{
				if (i + 1 < _path.Count)
				{
					_body[i].Position = _path[i + 1].Lerp(_path[i], lerpWeight);
				}
				else if (_path.Count > 0)
				{
					_body[i].Position = _path[^1];
				}
			}
		}
	}
}
