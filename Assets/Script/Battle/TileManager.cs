using System;
using System.Collections.Generic;
using CofyEngine;
using Otter.MonsterChess.Core;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoInstance<TileManager>
{
    [SerializeField] private Tilemap _tilemap;
    public Tilemap tilemap => _tilemap;

    public AxialTilemapData _tilemapData = new();

    protected override void Awake()
    {
        base.Awake();
        _tilemap = GetComponentInChildren<Tilemap>();
    }

    public void registerTile(HexTile tile, Action<AxialCoord> onRegister)
    {
        var cellIdx = _tilemap.WorldToCell(tile.transform.position);
        AxialCoord.OddRToAxial(cellIdx.x, cellIdx.y, out var axial);
        _tilemapData.addTile(axial, tile);
        onRegister(axial);
    }
}
