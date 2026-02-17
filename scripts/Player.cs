using Godot;
using System;
using MathNet.Numerics.Distributions;

public partial class Player : CharacterBody2D {
	Sprite2D Sprite;
	AnimationTree AnimTree;
	AnimState _AnimState = AnimState.Idle;
	AnimationNodeStateMachinePlayback StateMachine;

	enum AnimState {
		Walk = 1, Idle = 0
	}
	public static Player Instance;

	public override void _Ready() {
		Sprite = GetNode<Sprite2D>("Sprite");
		AnimTree = GetNode<AnimationTree>("AnimationTree");
		StateMachine = (AnimationNodeStateMachinePlayback)AnimTree.Get("parameters/StateMachine/playback");
		Instance = this;
	}

	Vector2 LastFrameDirection = Vector2.Zero;
	[Export(PropertyHint.Range, "50, 200, 1")]
	float Speed = 100;
	[Export(PropertyHint.Range, "5, 20, .1")]
	float TimeBetweenBlinks = 10;
	public override void _PhysicsProcess(double delta) {
		float horizontal = Input.GetAxis("Left", "Right");
		float vertical = Input.GetAxis("Down", "Up");
		Vector2 direction = new(horizontal, vertical);
		if (vertical != 0 && horizontal != 0) {
			if (LastFrameDirection.X == 0) {
				direction = direction.Project(Vector2.Up).Normalized();
			}
			else {
				direction = direction.Project(Vector2.Right).Normalized();
			}
		}
		Walk(direction);
		LastFrameDirection = direction;

		HandleInteractions();
	}

	public bool Interacting = false;
	void HandleInteractions() {
		if (!Input.IsActionJustPressed("Interact") || InteractionTarget == null) return;
		InteractionTarget.Interact();
		Interacting = true;
	}

	public void ResetBlink() {
		float currTime = Time.GetTicksMsec();
		Exponential distribution = new(1 / TimeBetweenBlinks);
		NextBlinkTime = currTime + (float)distribution.Sample() * 1000;
	}
	float NextBlinkTime = 0;
	void Walk(Vector2 direction) {
		if (direction == Vector2.Zero) {
			if (StateMachine.GetCurrentNode() == "Walk") {
				StateMachine.Travel("Idle");
				_AnimState = AnimState.Idle;
				ResetBlink();
			}
			else if (StateMachine.GetCurrentNode() == "Idle") {
				float currTime = Time.GetTicksMsec();
				if (currTime > NextBlinkTime) {
					StateMachine.Travel("Blink");
				}
			}
			return;
		}
		AnimTree.Set("parameters/StateMachine/Walk/blend_position", direction);
		AnimTree.Set("parameters/StateMachine/Idle/blend_position", direction);
		AnimTree.Set("parameters/StateMachine/Blink/blend_position", direction);
		if (StateMachine.GetCurrentNode() != "Walk") {
			StateMachine.Travel("Walk");
		}
		direction = direction.Project(Vector2.Up) * -2 + direction;
		Velocity = direction * Speed;
		MoveAndSlide();
		_AnimState = AnimState.Walk;
	}
	public InteractableComponent InteractionTarget = null;
	void FindInteractionTarget() {
		if (Interacting) return;
		float minDist = 0;
		InteractableComponent closestInteractable = null;
		float interactionMaxDistance = 45;
		foreach (InteractableComponent comp in InteractableComponent.AllInteractables) {
			Vector2 disp = comp.GlobalPosition - GlobalPosition;
			float dist = disp.Length();
			if (dist < interactionMaxDistance) {
				if (closestInteractable == null) {
					closestInteractable = comp;
					minDist = dist;
				}
				else if (dist < minDist) {
					closestInteractable = comp;
					minDist = dist;
				}
			}
		}
		InteractionTarget = closestInteractable;
	}

	public override void _Process(double delta) {
		FindInteractionTarget();
	}

}
