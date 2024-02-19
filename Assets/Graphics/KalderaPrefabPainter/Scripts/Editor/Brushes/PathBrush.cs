using CollisionBear.WorldEditor.Extensions;
using CollisionBear.WorldEditor.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CollisionBear.WorldEditor.Brushes
{
    [System.Serializable]
    public class PathBrush : BrushBase
    {
        private const float MaxSegmentLength = 0.25f;
        private const float BrushHandleInterval = 6f;

        [System.Serializable]
        public class PathBrushSettings
        {
            public float BrushSpacing = 1f;
            public bool OrientWithBrush = false;
        }

        public override string Name => "Path brush";
        public override KeyCode HotKey => KeyCode.E;
        public override string ToolTip => "Places multiple objects in a path along the mouse drag";

        protected override string ButtonImagePath => "Icons/IconPathTool.png";

        private List<Vector3> CurrentPath = new List<Vector3>();

        private Vector3? LastUserPosition;
        private int CurrentIndex;
        private int LastPlacementIndex;

        [SerializeField]
        private PathBrushSettings Settings = new PathBrushSettings();

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
            ResetDrag();

            StartDragPosition = position;
            EndDragPosition = position;
            BrushPosition = position;

            LastPlacementIndex = 0;

            AddFirstSegment(position);
        }

        public override List<GameObject> EndPlacement(Vector3 position, GameObject parentCollider, SelectionSettings settings, ScenePlacer placer)
        {
            ResetDrag();
            StartDragPosition = null;
            EndDragPosition = null;

            PruneEndPiece(placer.PlacementCollection);

            return base.EndPlacement(position, parentCollider, settings, placer);
        }

        public override void ActiveDragPlacement(Vector3 position, SelectionSettings selectionSettings, double deltaTime, ScenePlacer placer)
        {
            EndDragPosition = position;
            placer.UpdatePlacements();

            AddSegmentedPath(position, selectionSettings, placer);
        }

        private void AddFirstSegment(Vector3 position)
        {
            CurrentPath.Add(position);
            LastUserPosition = position;
        }

        private void AddSegmentedPath(Vector3 position, SelectionSettings settings, ScenePlacer placer)
        {
            var segments = SegmentPath(LastUserPosition.Value, position);

            foreach(var segmentPosition in segments) {
                AddSegment(segmentPosition, settings, placer);
            }

            LastUserPosition = position;
        }

        private List<Vector3> SegmentPath(Vector3 start, Vector3 end)
        {
            var result = new List<Vector3>();
            if(Vector3.Distance(start, end) == 0f) {
                return result;
            }

            var length = Vector3.Distance(start, end);

            if (length < MaxSegmentLength) {
                result.Add(end);
            } else {
                var segmentsRequired = Mathf.CeilToInt(length / MaxSegmentLength);
                var segmentLength = 1f / segmentsRequired;

                for (int i = 1; i <= segmentsRequired; i++) {
                    result.Add(Vector3.Lerp(start, end, i * segmentLength));
                }
            }

            return result;
        }

        private void AddSegment(Vector3 position, SelectionSettings settings, ScenePlacer placer)
        {
            CurrentPath.Add(position);
            UpdatePath(settings, placer.PlacementCollection);
        }

        public override void ShiftDragPlacement(Vector3 position, SelectionSettings settings, double deltaTime, ScenePlacer placer)
        {
            // Failesafe in case we get here
            if (!StartDragPosition.HasValue) {
                StartPlacement(position, placer);
                return;
            }

            var snappedPosition = GetRotationSnappedPosition(StartDragPosition.Value, position);
            ActiveDragPlacement(snappedPosition, settings, deltaTime, placer);
        }

        private void ResetDrag()
        {
            CurrentPath.Clear();
            CurrentIndex = 0;

            LastPlacementIndex = -1;
            LastUserPosition = null;
        }

        private void UpdatePath(SelectionSettings selectionSettings, PlacementCollection placementCollection)
        {
            var pointsToAdd = PointsForPath(selectionSettings.GetSelectedItemSize() * Settings.BrushSpacing);

            foreach (var point in pointsToAdd) {

                var placementInformation = CreatePlacementInformation(BrushPosition, GetOffsetPoint(point), selectionSettings);
                placementCollection.Placements.Add(placementInformation);
                LastPlacementIndex = placementCollection.Placements.IndexOf(placementInformation);
            }

            UpdateLastPlacement(placementCollection);
        }

        private void UpdateLastPlacement(PlacementCollection placementCollection)
        {
            if (LastPlacementIndex < 0) {
                return;
            }

            var placement = placementCollection.Placements[LastPlacementIndex];

            var startPosition = CurrentPath[CurrentIndex];
            var endPosition = CurrentPath.Last();
            UpdatePlacementPosition(placement, startPosition, endPosition);
        }

        private void UpdatePlacementPosition(PlacementInformation placement, Vector3 startPosition, Vector3 endPosition)
        {
            var position = Vector3.Lerp(startPosition, endPosition, 0.5f);
            placement.SetOffset(GetOffsetPoint(position));

            if (Settings.OrientWithBrush) {
                var rotation = placement.Rotation.eulerAngles;

                rotation.y = OrientWithBrushRotation(startPosition, endPosition, placement.Item);
                placement.SetRotation(rotation);
            }
        }

        private Vector2 GetOffsetPoint(Vector2 point) => point - new Vector2(BrushPosition.x, BrushPosition.z);

        private Vector3 GetOffsetPoint(Vector3 point) {
            var result = point - BrushPosition;
            result.y = 0;
            return result;
        }

        private List<Vector2> PointsForPath(float objectSpacing)
        {
            var result = new List<Vector2>();
            var lengthSinceLastPlacement = 0f;

            for (int i = Mathf.Max(CurrentIndex, 1); i < CurrentPath.Count; i++) {
                lengthSinceLastPlacement += Vector3.Distance(CurrentPath[i - 1], CurrentPath[i]);

                if (lengthSinceLastPlacement >= objectSpacing) {
                    lengthSinceLastPlacement -= objectSpacing;

                    var currentPosition = CurrentPath[CurrentIndex];
                    var newPosition = new Vector2(currentPosition.x, currentPosition.z);

                    // If we place multiple object the same frame, give them all the opportunity to center themselves
                    if (result.Count > 0) {
                        var lastIndex = result.Count - 1;
                        result[lastIndex] = Vector2.Lerp(result[lastIndex], newPosition, 0.5f); 
                    }

                    result.Add(newPosition);
                    CurrentIndex = i;
                }
            }

            return result;
        }

        private void PruneEndPiece(PlacementCollection placementCollection)
        {
            if(placementCollection.Placements.Count <= 1) {
                return;
            }

            var lastPlacement = placementCollection.Placements.Last();
            GameObject.DestroyImmediate(lastPlacement.GameObject);
            placementCollection.Placements.Remove(lastPlacement);
        }

        public override void DrawBrushHandle(Vector3 placementPosition, Vector3 mousePosition)
        {
            if (!HasDrag(StartDragPosition, EndDragPosition)) {
                return;
            }

            DrawIntialArrow();
            DrawFinalArrow();
        }

        private void DrawIntialArrow()
        {
            var rotation = Quaternion.identity;

            if(CurrentPath.Count > 1) {
                rotation = Quaternion.LookRotation((FindPointAtDistance(startIndex: 0, distance: BrushHandleInterval) - CurrentPath[0]).normalized) * QuaterRotation;
            } 

            DrawOnSceneViewMesh(KalderaEditorUtils.PlaneMesh, KalderaEditorUtils.ArrowMaterial, CurrentPath[0] - (rotation * Vector3.left * ArrowSize * 0.75f), rotation, new Vector3(ArrowSize, ArrowSize, ArrowSize));
        }

        private Vector3 FindPointAtDistance(int startIndex, float distance)
        {
            var currentIndex = startIndex;
            var currentLength = 0f;

            while (currentIndex < CurrentPath.Count -1 && currentLength < distance) {
                currentLength += Vector3.Distance(CurrentPath[currentIndex], CurrentPath[currentIndex + 1]);
                currentIndex++;
            }

            return CurrentPath[currentIndex];
        }

        private void DrawFinalArrow()
        {
            var position = CurrentPath.Last();

            var rotation = Quaternion.identity;
            var startPosition = FindPointAtDistanceBackwards(CurrentPath.Count - 1, BrushHandleInterval);
            if(Vector3.Distance(startPosition, position) > 0f) {
                rotation = Quaternion.LookRotation((startPosition - position).normalized) * Quaternion.Inverse(QuaterRotation);
            }

            DrawOnSceneViewMesh(KalderaEditorUtils.PlaneMesh, KalderaEditorUtils.LongArrowMaterial, position - (rotation * Vector3.left * ArrowSize * 0.75f), rotation, new Vector3(ArrowSize, ArrowSize, ArrowSize));
        }

        private Vector3 FindPointAtDistanceBackwards(int startIndex, float distance)
        {
            var currentIndex = startIndex;
            var currentLength = 0f;

            while (currentIndex > 2 && currentLength < distance) {
                currentLength += Vector3.Distance(CurrentPath[currentIndex], CurrentPath[currentIndex - 1]);
                currentIndex--;
            }

            return CurrentPath[currentIndex];
        }

        public override void DrawSceneHandleText(Vector2 screenPosition, Vector3 worldPosition, ScenePlacer placer)
        {
            var rotation = Quaternion.identity;

            if (EndDragPosition.HasValue && StartDragPosition.HasValue) {
                var dragDirection = GetDragDirection(EndDragPosition, EndDragPosition.Value);
                if (dragDirection.sqrMagnitude > 0f) {
                    rotation = Quaternion.LookRotation(dragDirection);
                }
            }

            try {
                DrawHandleTextAtOffset(screenPosition, 0, new GUIContent($"Rotation:\t {rotation.eulerAngles.y.ToString(RotationFormat)}"));
                DrawHandleTextAtOffset(screenPosition, 1, new GUIContent($"Object count: {placer.PlacementCollection.Placements.Count}"));
                DrawHandleTextAtOffset(screenPosition, 2, KalderaEditorUtils.MousePlaceToolTip);
            } catch (System.Exception e) {
                Debug.LogException(e);
            }
        }

        protected override List<Vector2> GetPlacementOffsetValues(Vector3 position, SelectionSettings selectionSettings, ScenePlacer placer) => EmptyPointList;

        private float OrientWithBrushRotation(Vector3 startPosition, Vector3 endPosition, PaletteItem item)
        {
            var direction = GetDragDirection(startPosition, endPosition);
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