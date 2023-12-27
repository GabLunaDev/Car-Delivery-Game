using System;
using Godot;

public partial class DeliveryCar : CharacterBody2D
{
    [ExportGroup("Style Properties")]
    [Export] public Texture2D CarSprite;

    [ExportGroup("Car Properties")]
    [Export(PropertyHint.Range, "100,1000,25")] public float EnginePower { get; set; } = 800f;
    [Export(PropertyHint.Range, "100,1000,25")] public float Braking { get; set; } = 450f;
    [Export(PropertyHint.Range, "100,1000,25")] public float MaxSpeedReverse { get; set; } = 250f;
    [Export(PropertyHint.Range, "0,100,1")] public float SteeringAngle = 20f;


    [ExportGroup("Physics Properties")]
    [Export(PropertyHint.Range, "-2,-0.1,0.1")] public float Friction { get; set; } = -0.9f;
    [Export(PropertyHint.Range, "-0.002,-0.0001,0.0001")] public float Drag { get; set; } = -0.0015f;
    [Export(PropertyHint.Range, "100,1000,25")] public float SlipSpeed { get; set; } = 400;
    [Export(PropertyHint.Range, "0.0001,1,0.0001")] public float TractionFast { get; set; } = 0.1f;
    [Export(PropertyHint.Range, "0.0001,1,0.0001")]public float TractionSlow { get; set; } = 0.7f; 

    private float _wheelBase = 177.5f;
    private Vector2 _acceleration = Vector2.Zero;
    private float _steerDirection;
    private float _forwardDirection;

    private void GetInput()
    {
        float turnDirection = Input.GetAxis("move_left", "move_right");
        _steerDirection = turnDirection * SteeringAngle;
        if(Input.IsActionPressed("move_forward")) 
            _acceleration = Transform.X * EnginePower * Input.GetActionStrength("move_forward");
        if(Input.IsActionPressed("move_backward"))
            _acceleration = Transform.X * -Braking * Input.GetActionStrength("move_backward");

    }

    private void CalculateSteering(double delta)
    {
        var rear_wheel = Position - Transform.X * _wheelBase/2;
        var front_wheel = Position + Transform.X * _wheelBase/2;

        rear_wheel += Velocity * (float)delta;
        front_wheel += Velocity.Rotated(_steerDirection) * (float)delta;
        var new_heading = (front_wheel - rear_wheel).Normalized();
        var traction = TractionSlow;
        if(Velocity.Length() > SlipSpeed)
            traction = TractionFast;
        var d = new_heading.Dot(Velocity.Normalized());
        if(d > 0)
            Velocity = Velocity.Lerp(new_heading * Velocity.Length(),traction);
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

    public override void _Ready()
    {
        var CarSpriteNode = GetNode<Sprite2D>("CarSprite");
        if(CarSprite != null)
            CarSpriteNode.Texture = CarSprite;
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
