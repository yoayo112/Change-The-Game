using CollisionBear.WorldEditor.Utils;
using UnityEngine;

namespace CollisionBear.WorldEditor.Brushes
{
    public class ClearButton : IBrushButton
    {
        private const string ButtonImagePath = "Icons/TrashIcon.png";

        public string Name => "Clear";
        public bool ShiftPrefix => false;
        public KeyCode HotKey => KeyCode.Space;
        public string ToolTip => "Clears the current selection and returns the Scene window to normal";
        public bool Disabled => false;

        private GUIContent ButtonContent;

        public GUIContent GetButtonContent()
        {
            if (ButtonContent == null) {
                ButtonContent = LoadGUIContent();
            }

            return ButtonContent;
        }

        public void OnButtonPress(PaletteWindow paletteWindow)
        {
            paletteWindow.ClearSelection();
        }

        public void OnSelected(ScenePlacer placer) { }

        protected virtual GUIContent LoadGUIContent()
        {
            var image = KalderaEditorUtils.LoadAssetPath(ButtonImagePath);
            return new GUIContent(image, $"{Name}\n{ToolTip}\n[{HotKey}]");
        }
    }
}