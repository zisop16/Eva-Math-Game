using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class InteractableComponent : Node2D {
	[Export]
	Sprite2D InteractionTarget;
	ShaderMaterial ShaderMat;

	public static List<InteractableComponent> AllInteractables = [];


	public override void _Ready() {
		AllInteractables.Add(this);
	}

	public override void _PhysicsProcess(double delta) {
		bool shouldHighlight = (Player.Instance.InteractionTarget == this) && (!Player.Instance.Interacting);
		HighlightInteraction(shouldHighlight);
	}

	bool Highlighted = false;
	void HighlightInteraction(bool flag) {
		if (Highlighted == flag) return;
		InteractionTarget.SetInstanceShaderParameter("toggled", flag);
		Highlighted = flag;
	}

	public void Interact() {
		GetParent<Interactable>().Interact();
	}
}

public interface Interactable {
	public void Interact();
	public void EndInteraction();
}