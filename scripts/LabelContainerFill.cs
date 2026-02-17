using Godot;
using System;

[Tool]
[GlobalClass]
public partial class LabelContainerFill : Container {
	Label Child;
	public override void _Ready() {
		CallDeferred(MethodName.AfterReady);
	}
	void AfterReady() {
		PreSortChildren += AdjustLabelFontSize;
		Child = GetChild<Label>(0);
	}

	void AdjustLabelFontSize() {
		Font font = Child.GetThemeFont("font");
		int currFontSize = (int)Mathf.Floor(Size.Y) * 2;
		Vector2 textSize;
		while (true) {
			textSize = font.GetStringSize(Child.Text, HorizontalAlignment.Left, -1, currFontSize);
			if (textSize.X > Size.X) {
				currFontSize -= 2;
			}
			else {
				break;
			}
		}
		Child.AddThemeFontSizeOverride("font_size", currFontSize);
		FitChildInRect(Child, new Rect2(Vector2.Zero, Size));
	}

}
