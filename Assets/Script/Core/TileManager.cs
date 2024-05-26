using System;
using System.Collections.Generic;
using CofyEngine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoInstance<TileManager>
{
    [SerializeField] private Tilemap _grid;
    
    private Dictionary<AxialCoord, HexTile> _tileRegistry = new();

    protected override void Awake()
    {
        base.Awake();
        _grid = GetComponentInChildren<Tilemap>();
    }

    public void registerTile(HexTile tile, Action<AxialCoord> onRegister)
    {
        var cellIdx = _grid.WorldToCell(tile.transform.position);
        AxialCoord.OddRToAxial(cellIdx.x, cellIdx.y, out var axial);
        _tileRegistry.Add(axial, tile);
        onRegister(axial);
    }
}
