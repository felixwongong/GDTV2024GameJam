using System.Collections.Generic;
using System.Linq;

namespace Otter.MonsterChess.Core
{
    public class AxialTilemapData
    {
        private Dictionary<AxialCoord, HexTile> _mapData = new();

        public int count => _mapData.Count;

        public bool tryGetTile(AxialCoord coord, out HexTile tile)
        {
            return _mapData.TryGetValue(coord, out tile);
        }

        public HexTile getTile(AxialCoord coord)
        {
            if(!_mapData.TryGetValue(coord, out var tile))
                return null;
            return tile;
        }

        public void clear()
        {
            _mapData.Clear();
        }

        public void addTile(AxialCoord coord, HexTile tile)
        {
            _mapData.Add(coord, tile);
        }
        
        public bool tryAddTile(AxialCoord coord, HexTile tile) {
            return _mapData.TryAdd(coord, tile);
        }

        public bool hasTile(AxialCoord coord)
        {
            return _mapData.ContainsKey(coord);
        }
        
        public AxialCoord[] getAllTilesCoord()
        {
            return _mapData.Keys.ToArray();
        }
    }
}