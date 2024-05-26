using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace Core.Skill.AxialArea
{
    public enum AreaTileType
    {
        Origin,
        Target,
        Destination
    }

    [CreateAssetMenu(fileName = "AreaTile", menuName = "Area/_Tile", order = 999)]
    public class AreaTile : Tile
    {
        [SerializeField] public AreaTileType type;
    }
}