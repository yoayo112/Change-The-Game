using CollisionBear.WorldEditor.Distribution;
using CollisionBear.WorldEditor.Generation;
using CollisionBear.WorldEditor.Utils;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Brushes
{
    [System.Serializable]
    public class SprayBrush : BrushBase
    {
        public const float SprayIntensityMin = 1f;
        public const float SprayIntensityMax = 100f;

        [System.Serializable]
        public class SprayBrushSettings
        {
            public float BrushSize = AreaBrushSettings.BrushSizePresets[1].BrushSize;
            public float ObjectDensity = 1.0f;
            public float SprayIntensity = 10.0f;
        }

        private double TimeToNextStroke;
        public override string Name => "Spray brush";
        public override KeyCode HotKey => KeyCode.Y;
        public override string ToolTip => "Slowly plots down objects while keeping the mouse button pressed";

        protected override string ButtonImagePath => "Icons/IconGridSpray.png";

        [SerializeField]
        private SprayBrushSettings Settings = new SprayBrushSettings();

        private List<GameObject> PlacedGameObjects = new List<GameObject>();
        private IGenerationBounds GenerationBounds;

        public SprayBrush()
        {
            GenerationBounds = new CircleBrush.CircleGenerationBounds();
        }

        public override void DrawBrushEditor(ScenePlacer placer)
        {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(KalderaEditorUtils.BrushSizeContent, GUILayout.Width(KalderaEditorUtils.OptionLabelWidth));
                var tmpBrushSize = EditorGUILayout.Slider(Settings.BrushSize, AreaBrushBase.BrushSizeMin, AreaBrushBase.BrushSizeMax);
                if (tmpBrushSize != Settings.BrushSize) {
                    SetBrushSize(tmpBrushSize, placer);
                }
            }

            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(KalderaEditorUtils.ObjectDensityContent, GUILayout.Width(KalderaEditorUtils.OptionLabelWidth));
                var tmpBrushSpacing = EditorGUILayout.Slider(Settings.ObjectDensity, AreaBrushBase.BrushSpacingMin, AreaBrushBase.BrushSpacingMax);
                if (tmpBrushSpacing != Settings.ObjectDensity) {
                    Settings.ObjectDensity = tmpBrushSpacing;
                    placer.NotifyChange();
                }
            }

            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(KalderaEditorUtils.SprayIntensityContent, GUILayout.Width(KalderaEditorUtils.OptionLabelWidth));
                var tmpSprayIntensity = EditorGUILayout.Slider(Settings.SprayIntensity, SprayIntensityMin, SprayIntensityMax);
                if (tmpSprayIntensity != Settings.SprayIntensity) {
                    Settings.SprayIntensity = tmpSprayIntensity;
                    placer.NotifyChange();
                }
            }
        }

        protected override List<Vector2> GetPlacementOffsetValues(Vector3 position, SelectionSettings selectionSettings, ScenePlacer placer)
        {
            var spacing = selectionSettings.GetSelectedItemSize() * (1f / Settings.ObjectDensity);
            return new RandomDistribution.GeneratedPositions(Settings.BrushSize, spacing, GenerationBounds).GetPoints();
        }

        protected override List<PlacementInformation> PlacementsToPlace(ScenePlacer placer)
        {
            return new List<PlacementInformation> { placer.PlacementCollection.Placements[Random.Range(0, placer.PlacementCollection.Placements.Count)] };
        }

        public override void DrawBrushHandle(Vector3 placementPosition, Vector3 mousePosition)
        {
            Handles.color = HandleBrushColor;
            Handles.DrawSolidDisc(placementPosition, Vector3.up, Settings.BrushSize);

            Handles.color = HandleOutlineColor;
            Handles.DrawWireDisc(placementPosition, Vector3.up, Settings.BrushSize);
        }

        public override bool HandleKeyEvents(Event currentEvent, ScenePlacer placer)
        {
            if (currentEvent.type == EventType.KeyDown && currentEvent.shift) {
                foreach (var preset in AreaBrushSettings.BrushSizePresets) {
                    if (currentEvent.keyCode == preset.Hotkey) {
                        SetBrushSize(preset.BrushSize, placer);
                        return true;
                    }
                }
            }

            return false;
        }

        public override void StartPlacement(Vector3 position, ScenePlacer placer)
        {
            PlacedGameObjects.Clear();
            base.StartPlacement(position, placer);
        }

        public override void ActiveDragPlacement(Vector3 worldPosition, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            SprayPlacement(worldPosition, settings, deltaTime, placer);
        }

        public override void StaticDragPlacement(Vector3 position, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            SprayPlacement(position, settings, deltaTime, placer);
        }

        private void SprayPlacement(Vector3 worldPosition, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            TimeToNextStroke -= deltaTime;
            placer.MovePosition(placer.ScreenPosition, worldPosition);

            if (TimeToNextStroke > 0) {
                return;
            }

            PlaceObject(worldPosition, settings, placer);
        }

        private void PlaceObject(Vector3 worldPosition, SelectionSettings settings, ScenePlacer placer)
        {
            var placeObjectInterval = GetObjectIntervalInSeconds();

            TimeToNextStroke += placeObjectInterval;
            PlacedGameObjects.AddRange(PlaceObjects(worldPosition, null, settings, placer));
            placer.GeneratePlacement();
        }

        private float GetObjectIntervalInSeconds() => 1f / Settings.SprayIntensity;

        public override List<GameObject> EndPlacement(Vector3 position, GameObject parentCollider, SelectionSettings settings, ScenePlacer placer)
        {
            return PlacedGameObjects;
        }

        private void SetBrushSize(float size, ScenePlacer placer)
        {
            Settings.BrushSize = size;
            placer.NotifyChange();
        }
    }
}