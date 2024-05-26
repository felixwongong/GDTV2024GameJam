using System.Collections.Generic;
using System.Linq;
using Core.Skill.AxialArea;
using Otter.MonsterChess.Skill;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Otter.MonsterChess.Core
{
    [CustomGridBrush(true, false, false, "AxialAreaBrush")]
    public class AxialAreaBrush: GridBrushBase
    {
        [SerializeField] private GridLayout _gridAsset;
        [HideInInspector] public GridLayout gridInstance;
        [SerializeField] public CustomArea _areaSO;
        [SerializeField] private TileBase _boardDefaultTile;
        [SerializeField] public AreaTile _originTile;
        [SerializeField] public AreaTile _targetTile;
        [SerializeField] public Direction _direction = Direction.Top;

        [HideInInspector]
        public AreaTile activePaintingTile = null;

        private static readonly Vector2Int vec2IntMin = new Vector2Int(int.MinValue, int.MinValue);
        
        public void createBoard()
        {
            if (gridInstance == null)
                gridInstance = GameObject.Find("_AxialAreaBrush_Grid")?.GetComponentInChildren<Grid>();
            if (gridInstance == null)
            {
                var grid = Instantiate(_gridAsset);
                grid.name = "_AxialAreaBrush_Grid";
                gridInstance = grid;
                grid.gameObject.hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
            }
        }

        public void redraw()
        {
            createBoard();
            if (_areaSO != null)
            {
                _direction = Direction.Top;
                draw(AxialCoord.zero, _areaSO.offsets, _direction);
            }
        }

        private void draw(AxialCoord origin, List<AxialCoord> offsets, Direction direction)
        {
            var tilemap = gridInstance.GetComponentInChildren<Tilemap>();
            paintOrigin(tilemap, origin, direction);

            foreach (var offset in offsets)
            {
                if (offset.oddr() == Vector3Int.zero) continue;

                tilemap.SetTile(offset.oddr() + origin.oddr(), _targetTile);
            }
        }

        private void paintOrigin(Tilemap tilemap, AxialCoord position, Direction direction)
        {
            var changeData = new TileChangeData()
            {
                color = _originTile.color,
                position = position.oddr(),
                tile = _originTile,
                transform = Matrix4x4.Rotate(direction.coord().getRotation())
            };
            tilemap.SetTile(changeData, false);
        }

        public void clearBoard()
        {
            var tilemap = gridInstance.GetComponentInChildren<Tilemap>();
            var bound = tilemap.cellBounds;

            for (int i = bound.xMin; i < bound.xMax; i++)
            {
                for (int j = bound.yMin; j < bound.yMax; j++)
                {
                    var pos = new Vector3Int(i, j, 0);
                    if (tilemap.GetTile(pos) is AreaTile)
                    {
                        tilemap.SetTile(pos, _boardDefaultTile);
                    }
                }
            }
        }
        
        public void rotateBoardCounterClockwise()
        {
            if (!fetchBoardData(out var origin, out var oddrOffsets)) return;
            clearBoard();
            
            var newDirection = (Direction)((byte)(_direction + 1) % 6);
            var axialOffsets = oddrOffsets.Select(oddr => oddr.axial().rotate60Counter()).ToList();
            draw(origin.axial(), axialOffsets, newDirection);
            _direction = newDirection;
        }
        
        public void removeBoard()
        {
            GameObject go;
            if (gridInstance == null)
            {
                go = GameObject.Find("_AxialAreaBrush_Grid");
                if (go != null) gridInstance = go.GetComponentInChildren<Grid>();
            }

            if (gridInstance != null)
            {
                go = gridInstance.gameObject;
                gridInstance = null;
                DestroyImmediate(go);
            }
        }

        public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.Paint(gridLayout, brushTarget, position);
            
            var tilemap = brushTarget.GetComponentInChildren<Tilemap>();

            if (activePaintingTile == _originTile)
            {
                paintOrigin(tilemap, position.xy().axial(), _direction);
            }
            else
            {
                tilemap.SetTile(position, activePaintingTile);
            }
        }

        public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
        {
            base.Erase(gridLayout, brushTarget, position);
            
            var tilemap = brushTarget.GetComponentInChildren<Tilemap>();
            tilemap.SetTile(position, null);
        }

        public void setAreaSO()
        {
            _areaSO.offsets.Clear();
            
            if (!fetchBoardData(out var origin, out var oddrOffsets)) return;

            _areaSO.offsets.Add(Vector2Int.zero.axial());
            foreach (var offset in oddrOffsets)
            {
                _areaSO.offsets.Add(offset.axial().rotateToTop(_direction.coord()));
            }
        }

        private bool fetchBoardData(out Vector2Int origin, out List<Vector2Int> oddrOffsets)
        {
            var tilemap = gridInstance.GetComponentInChildren<Tilemap>();
            var bound = tilemap.cellBounds;

            origin = vec2IntMin;
            oddrOffsets = new();
            for (int i = bound.xMin; i < bound.xMax; i++)
            {
                for (int j = bound.yMin; j < bound.yMax; j++)
                {
                    var pos = new Vector3Int(i, j, 0);
                    var tile = tilemap.GetTile(pos) as AreaTile;
                    if (tile == null) continue;
                    switch (tile.type)
                    {
                        case AreaTileType.Origin:
                            if (origin != vec2IntMin)
                            {
                                EditorUtility.DisplayDialog("Error", "More than 1 origin, aborted", "OK");
                                return true;
                            }

                            origin = pos.xy();
                            break;
                        case AreaTileType.Target:
                            oddrOffsets.Add(pos.xy());  //this is world position without offset
                            break;
                    }
                }
            }
            
            if (origin == vec2IntMin)
            {
                EditorUtility.DisplayDialog("Error", "Origin not found", "OK");
                return false;
            }

            var _origin = origin;
            oddrOffsets = oddrOffsets.Select(offset => offset - _origin).ToList();


            return true;
        }
    }
}