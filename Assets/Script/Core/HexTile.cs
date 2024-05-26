using UnityEngine;

public class HexTile : MonoBehaviour
{
    [SerializeField] private AxialCoord _coord;
    
    
    private void Start()
    {
        TileManager.instance.registerTile(this, coord => _coord = coord);
    }
}
