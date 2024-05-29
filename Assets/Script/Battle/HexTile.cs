using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    [SerializeField] private AxialCoord _coord;
    [SerializeField] private UnitTeam _team;
    [SerializeField] private List<GameObject> tiles;

    private Unit occupiedUnit;
    public UnitTeam team => _team;

    private void Start()
    {
        TileManager.instance.registerTile(this, coord => _coord = coord);
        
        foreach (var tile in tiles)
        {
            tile.gameObject.SetActive(false);
        }
        
        changeTeam(UnitTeam.None, UnitTeam.None);
    }

    public void occupyBy(int unitId)
    {
        var unit = BattleController.instance.unitMap[unitId];
        changeTeam(occupiedUnit != null ? occupiedUnit.team : UnitTeam.None, unit.team);
        this.occupiedUnit = unit;
    }

    private void changeTeam(UnitTeam from, UnitTeam to)
    {
        var fromTile = tiles.Find(tile => tile.name == from.ToString());
        var toTile = tiles.Find(tile => tile.name == to.ToString());
        
        if (fromTile != null)
        {
            fromTile.SetActive(false);
        }
        
        toTile.gameObject.SetActive(true);
    }
}
