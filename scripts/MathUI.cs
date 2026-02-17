using Godot;
using System;

public partial class MathUI : Control {
	public enum Operation {
		Add, Subtract, Multiply, Divide
	}
	Label Num1;
	Label Num2;
	Label Operator;

	public override void _Ready() {
		Num1 = GetNode<Label>("%Num1");
		Num2 = GetNode<Label>("%Num2");
		Operator = GetNode<Label>("%Operator");
	}

	public void LoadOperation(int num1, int num2, Operation operation) {
		Num1.Text = num1.ToString();
		Num2.Text = num2.ToString();
		switch (operation) {
			case Operation.Add: {
					Operator.Text = "+";
					break;
				}
			case Operation.Multiply: {
					Operator.Text = "X";
					break;
				}
			case Operation.Subtract: {
					Operator.Text = "-";
					break;
				}
			case Operation.Divide: {
					Operator.Text = "÷";
					break;
				}
		}

	}
}
