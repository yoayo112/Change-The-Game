using UnityEngine;

namespace CollisionBear.WorldEditor.Brushes
{
    public interface IBrushButton
    {
        string Name { get; }
        bool ShiftPrefix {  get; }
        KeyCode HotKey { get; }
        string ToolTip { get; }
        bool Disabled { get; }

        GUIContent GetButtonContent();
        void OnButtonPress(PaletteWindow paletteWindow);
        void OnSelected(ScenePlacer placer);
    }
}