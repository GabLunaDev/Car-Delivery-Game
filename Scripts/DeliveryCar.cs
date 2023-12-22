using Godot;

public partial class DeliveryCar : CharacterBody2D
{
    [Export]
    public int Speed { get; set; } = 400;

    [Export]
    public float RotationSpeed { get; set; } = 0.5f;

    private float _rotationDirection;
    private float _forwardDirection;

    public void GetInput()
    {
        _rotationDirection = Input.GetAxis("move_left", "move_right");
        _forwardDirection = Input.GetAxis("move_backward", "move_forward");
    }

    public override void _PhysicsProcess(double delta)
    {
        GetInput();

        int SpeedValue = 0;

        if(_forwardDirection > 0)
            SpeedValue = Speed;
            if(Velocity.X != 0)
                Rotation += _rotationDirection * RotationSpeed * 3 * (float)delta;    
        if(_forwardDirection < 0)
            SpeedValue = Speed/2;
            if(Velocity.X != 0)
                Rotation += _rotationDirection * RotationSpeed * 0.5f * (float)delta;

        Velocity = Transform.X * _forwardDirection * SpeedValue;

        GD.Print(Rotation);
        
        MoveAndSlide();
    }
}
