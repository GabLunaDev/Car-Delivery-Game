using System;
using Godot;

public partial class DeliveryCar : CharacterBody2D
{
    [Export] public float EnginePower { get; set; } = 800f;
    [Export] public float Braking { get; set; } = -450f;
    [Export] public float MaxSpeedReverse { get; set; } = 250f;
    [Export] public float Friction { get; set; } = -0.9f;
    [Export] public float Drag { get; set; } = -0.0015f;


    private float _steeringAngle = 20f;
    private float _wheelBase = 177.5f;
    private Vector2 _velocity = Vector2.Zero;
    private Vector2 _acceleration = Vector2.Zero;
    private float _steerDirection;
    private float _forwardDirection;

    private void GetInput()
    {
        float turnDirection = Input.GetAxis("move_left", "move_right");
        _steerDirection = turnDirection * _steeringAngle;
        if(Input.IsActionPressed("move_forward")) 
            _acceleration = Transform.X * EnginePower * Input.GetActionStrength("move_forward");
        if(Input.IsActionPressed("move_backward"))
            _acceleration = Transform.X * Braking * Input.GetActionStrength("move_backward");

    }

    private void CalculateSteering(double delta)
    {
        var rear_wheel = Position - Transform.X * _wheelBase/2;
        var front_wheel = Position + Transform.X * _wheelBase/2;

        rear_wheel += Velocity * (float)delta;
        front_wheel += Velocity.Rotated(_steerDirection) * (float)delta;
        var new_heading = (front_wheel - rear_wheel).Normalized();
        var d = new_heading.Dot(Velocity.Normalized());
        if(d > 0)
            Velocity = new_heading * Velocity.Length();
        if(d < 0)
            Velocity = -new_heading * Math.Min(Velocity.Length(), MaxSpeedReverse);
        Rotation = new_heading.Angle();
    }

    private void ApplyFriction()
    {
        if(Velocity.Length() < 5)
            Velocity = Vector2.Zero;
        var frictionForce = Velocity * Friction;
        var dragForce = Velocity * Velocity.Length() * Drag;

        if(Velocity.Length() < 100)
            frictionForce *= 3;
        _acceleration += dragForce + frictionForce;
    }

    public override void _PhysicsProcess(double delta)
    {
        _acceleration = Vector2.Zero;
        GetInput();
        CalculateSteering(delta);
        ApplyFriction();
        Velocity += _acceleration * (float)delta;
        MoveAndSlide();
    }
}
