using CollisionBear.WorldEditor.Extensions;
using CollisionBear.WorldEditor.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Brushes
{
    [System.Serializable]
    public class LineBrush : BrushBase
    {
        [System.Serializable]
        public class LineBrushSettings
        {
            public float BrushSpacing = 1f;
            public bool OrientWithBrush = false;
        }

        public override string Name => "Line brush";
        public override KeyCode HotKey => KeyCode.W;
        public override string ToolTip => "Places multiple objects in a line from where you start drag to the end";

        protected override string ButtonImagePath => "Icons/IconGridLine.png";

        [SerializeField]
        private LineBrushSettings Settings = new LineBrushSettings();

        public override void DrawBrushEditor(ScenePlacer placer)
        {
            using (new EditorGUILayout.HorizontalScope()) {
                EditorGUILayout.LabelField(KalderaEditorUtils.ObjectDensityContent, GUILayout.Width(KalderaEditorUtils.OptionLabelWidth));
                var tmpBrushSpacing = EditorGUILayout.Slider(Settings.BrushSpacing, AreaBrushBase.BrushSpacingMin, AreaBrushBase.BrushSpacingMax);
                if (tmpBrushSpacing != Settings.BrushSpacing) {
                    Settings.BrushSpacing = tmpBrushSpacing;
                    placer.NotifyChange();
                }
            }

            using (new EditorGUILayout.HorizontalScope()) {
                using (new EditorGUILayout.HorizontalScope()) {
                    EditorCustomGUILayout.SetGuiBackgroundColorState(Settings.OrientWithBrush);
                    if (GUILayout.Button(KalderaEditorUtils.OrientWithBrushContent, GUILayout.Width(KalderaEditorUtils.IconButtonSize), GUILayout.Height(KalderaEditorUtils.IconButtonSize))) {
                        Settings.OrientWithBrush = !Settings.OrientWithBrush;
                        placer.NotifyChange();
                    }
                }
            }
        }

        public override void StartPlacement(Vector3 position, ScenePlacer placer)
        {
            StartDragPosition = position;
            EndDragPosition = position;
            BrushPosition = position;
        }

        public override List<GameObject> EndPlacement(Vector3 position, GameObject parentCollider, SelectionSettings settings, ScenePlacer placer)
        {
            StartDragPosition = null;
            EndDragPosition = null;
            return base.EndPlacement(position, parentCollider, settings, placer);
        }

        public override void ActiveDragPlacement(Vector3 position, SelectionSettings selectionSettings, double deltaTime, ScenePlacer placer)
        {
            EndDragPosition = position;
            placer.UpdatePlacements();

            var objectCount = PointCountToGenerate(StartDragPosition.Value, position, selectionSettings.GetSelectedItemSize() * Settings.BrushSpacing);
            var currenPlacementCount = placer.PlacementCollection.Placements.Count;

            if (objectCount < currenPlacementCount) {
                PruneToObjectCount(objectCount, placer.PlacementCollection);
            } else if (objectCount >= placer.PlacementCollection.Placements.Count) {
                UpdateAndFillUpToCount(StartDragPosition.Value, position, currenPlacementCount, selectionSettings, placer.PlacementCollection);
            }
        }

        public override void ShiftDragPlacement(Vector3 position, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            // Failesafe in case we get here
            if(!StartDragPosition.HasValue) {
                StartPlacement(position, placer);
                return;
            }

            var snappedPosition = GetRotationSnappedPosition(StartDragPosition.Value, position);
            ActiveDragPlacement(snappedPosition, settings, deltaTime, placer);
        }

        private void PruneToObjectCount(int objectCount, PlacementCollection placementCollection)
        {
            for (int i = placementCollection.Placements.Count; i > objectCount; i--) {
                placementCollection.Placements.Last().ClearPlacementGameObject();
                placementCollection.Placements.RemoveAt(placementCollection.Placements.Count - 1);
            }
        }

        private void UpdateAndFillUpToCount(Vector3 start, Vector3 end, int currentObjectCount, SelectionSettings selectionSettings, PlacementCollection placementCollection)
        {
            var offsetsToGenerate = PointsForLine(start, end, selectionSettings.GetSelectedItemSize() * Settings.BrushSpacing);

            for (int i = 0; i < offsetsToGenerate.Count; i++) {
                if (i < currentObjectCount) {
                    var offset = new Vector3(offsetsToGenerate[i].x, 0, offsetsToGenerate[i].y);
                    placementCollection.Placements[i].SetOffset(offset);
                } else {
                    var placementInformation = CreatePlacementInformation(BrushPosition, offsetsToGenerate[i], selectionSettings);
                    placementCollection.Placements.Add(placementInformation);
                }

                if (Settings.OrientWithBrush) {
                    var placement = placementCollection.Placements[i];

                    var rotation = placement.Rotation.eulerAngles;
                    rotation.y = OrientWithBrushRotation(end, placement.Item);
                    placement.SetRotation(rotation);
                }
            }
        }

        public override void DrawBrushHandle(Vector3 placementPosition, Vector3 mousePosition)
        {
            if (HasDrag(StartDragPosition, EndDragPosition)) {
                DrawLineArrow(StartDragPosition.Value, EndDragPosition.Value);
            }
        }

        public override void DrawSceneHandleText(Vector2 screenPosition, Vector3 worldPosition, ScenePlacer placer)
        {
            var rotation = Quaternion.identity;

            if (EndDragPosition.HasValue && StartDragPosition.HasValue) {
                var dragDirection = GetDragDirection(EndDragPosition, StartDragPosition.Value);
                if (dragDirection.sqrMagnitude > 0f) {
                    rotation = Quaternion.LookRotation(dragDirection);
                }
            }

            try {
                DrawHandleTextAtOffset(screenPosition, 0, new GUIContent($"Rotation:\t {rotation.eulerAngles.y.ToString(RotationFormat)}"));
                DrawHandleTextAtOffset(screenPosition, 1, new GUIContent($"Object count: {placer.PlacementCollection.Placements.Count}"));
                DrawHandleTextAtOffset(screenPosition, 2, KalderaEditorUtils.MousePlaceToolTip);
            }catch(System.Exception e) {
                Debug.LogException(e);
            }
        }

        protected override List<Vector2> GetPlacementOffsetValues(Vector3 position, SelectionSettings selectionSettings, ScenePlacer placer)
        {
            return EmptyPointList;
        }

        private int PointCountToGenerate(Vector3 start, Vector3 end, float objectSpacing)
        {
            // See how many objects we can fit on the line, with a minimum of one
            var lineLength = (end - start).magnitude;
            return Mathf.Max(1, Mathf.CeilToInt(lineLength / objectSpacing));
        }

        private List<Vector2> PointsForLine(Vector3 start, Vector3 end, float objectSpacing)
        {
            var result = new List<Vector2>();

            var direction = (end - start).normalized;
            var objectCount = PointCountToGenerate(start, end, objectSpacing);
            var individualOffset = 1f / objectCount;

            var directionalEnd = objectCount * objectSpacing * direction;
            for (int i = 0; i < objectCount; i++) {
                var offset = Vector3.Lerp(Vector3.zero, directionalEnd, i * individualOffset);
                result.Add(new Vector2(offset.x, offset.z));
            }

            return result;
        }

        protected override Vector3 GetItemRotation(Vector3 position, PaletteItem item, GameObject prefabObject)
        {
            var result = base.GetItemRotation(position, item, prefabObject);

            if (Settings.OrientWithBrush) {
                result.y = OrientWithBrushRotation(position, item);
            }

            return result;
        }

        private float OrientWithBrushRotation(Vector3 endPosition, PaletteItem item)
        {
            var direction = GetDragDirection(StartDragPosition, endPosition);
            var rotationToEnd = direction.DirectionToPerpendicularRotationY();
            return rotationToEnd + item.AdvancedOptions.RotationOffset.y;
        }

        private Vector3 GetDragDirection(Vector3? startDragPosition, Vector3 placementPosition)
        {
            if (startDragPosition.HasValue) {
                return (startDragPosition.Value - placementPosition).normalized;
            } else {
                return Vector3.zero;
            }
        }
    }
}