using Core.Skill.AxialArea;
using Otter.MonsterChess.Skill;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Otter.MonsterChess.Core
{
    [CustomEditor(typeof(AxialAreaBrush))]
    public class AxialAreaBrushEditor : GridBrushEditorBase
    {
        private AxialAreaBrush brush => target as AxialAreaBrush;

        public override GameObject[] validTargets
        {
            get
            {
                if (brush.gridInstance != null)
                {
                    return new[] { brush.gridInstance.GetComponentInChildren<Tilemap>().gameObject };
                }

                return null;
            }
        }

        private void OnEnable()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.EnteredPlayMode)
            {
                brush.removeBoard();
            }
        }


        public override void OnToolActivated(GridBrushBase.Tool tool)
        {
            base.OnToolActivated(tool);
            brush.createBoard();
        }

        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position,
            GridBrushBase.Tool tool, bool executing)
        {
            var gizmoRect = position;

            if (tool == GridBrushBase.Tool.Paint || tool == GridBrushBase.Tool.Erase)
                gizmoRect = new BoundsInt(position.min, Vector3Int.one);

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            if (Event.current.type == EventType.DragExited)
            {
                brush.activePaintingTile = brush._targetTile;
                var drags = DragAndDrop.objectReferences;
                if (drags is { Length: > 0 })
                {
                    foreach (var drag in drags)
                    {
                        if (drag is not AxialArea dragArea)
                        {
                            EditorUtility.DisplayDialog("Error", "Dragged object is not an AxialArea", "OK");
                            break;
                        }

                        var area = dragArea.getArea(position.position.xy().axial(), brush._direction.coord());
                        foreach (var coord in area)
                        {
                            brush.Paint(gridLayout, brushTarget, coord.oddr());
                        }
                    }
                }
            }

            base.OnPaintSceneGUI(gridLayout, brushTarget, gizmoRect, tool, executing);
        }

        public AreaTileType tileType;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            tileType = (AreaTileType)EditorGUILayout.EnumPopup("Active painting", tileType);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Board"))
            {
                brush.clearBoard();
            }

            if (GUILayout.Button("Rotate Board"))
            {
                brush.rotateBoardCounterClockwise();
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set to Area SO"))
            {
                brush.setAreaSO();
            }

            if (GUILayout.Button("Redraw Area SO"))
            {
                brush.removeBoard();
                brush.createBoard();
                brush.redraw();
            }
            EditorGUILayout.EndHorizontal();

            if (brush._areaSO != null)
            {
                var serializedArea = new SerializedObject(brush._areaSO);
                EditorGUILayout.PropertyField(serializedArea.FindProperty("offsets"), true);
                serializedArea.ApplyModifiedProperties();
            }

            switch (tileType)
            {
                case AreaTileType.Origin:
                    brush.activePaintingTile = brush._originTile;
                    break;
                case AreaTileType.Target:
                    brush.activePaintingTile = brush._targetTile;
                    break;
                case AreaTileType.Destination:
                    brush.activePaintingTile = null;
                    break;
            }
        }
    }
}