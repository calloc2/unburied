using Godot;

public partial class Camera : Camera3D
{
	// Proprerties
	
	private const float MIN_ZOOM = 5.0f;
	private const float MAX_ZOOM = 15.0f;

	[Export] public float ZoomSpeed = 0.5f;

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseButtonEvent)
		{
			if (mouseButtonEvent.ButtonIndex == MouseButton.WheelDown)
				Size = Mathf.Clamp(Size + ZoomSpeed, MIN_ZOOM, MAX_ZOOM);

			if (mouseButtonEvent.ButtonIndex == MouseButton.WheelUp)
				Size = Mathf.Clamp(Size - ZoomSpeed, MIN_ZOOM, MAX_ZOOM);
		}
	}
}