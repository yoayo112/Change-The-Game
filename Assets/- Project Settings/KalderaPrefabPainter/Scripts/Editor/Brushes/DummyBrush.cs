using System.Collections.Generic;
using UnityEngine;

namespace CollisionBear.WorldEditor.Brushes
{
    [System.Serializable]
    public class DummyBrush : BrushBase
    {
        private string BrushName;
        private KeyCode BrushHotKey;
        private string BrushToolTip;
        private string ImagePath;

        public override string Name => BrushName;

        public override KeyCode HotKey => BrushHotKey;

        public override string ToolTip => BrushToolTip;

        protected override string ButtonImagePath => ImagePath;

        public override bool Disabled => true;

        public DummyBrush(string brushName, KeyCode hotkey, string toolTip, string imagePath)
        {
            BrushName = brushName;
            BrushHotKey = hotkey;
            BrushToolTip = toolTip;
            ImagePath = imagePath;
        }

        public override void DrawBrushEditor(ScenePlacer placer) { }

        protected override List<Vector2> GetPlacementOffsetValues(Vector3 position, SelectionSettings selectionSettings, ScenePlacer placer) => EmptyPointList;
    }
}
