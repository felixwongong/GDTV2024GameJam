using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BattleController : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPositions;

    private static BattleController _instance;
    public static BattleController instance => _instance;
    
    [SerializeField]
    private NetworkVariable<int> idGenerator = new();

    public int generateId
    {
        get
        {
            var id = idGenerator.Value;
            incrementIdRpc();
            return id;
        }
    }

    [Rpc(SendTo.Server)]
    void incrementIdRpc()
    {
        idGenerator.Value += 1;
    }
    
    private void Awake()
    {
        _instance = this;
    }

    public Transform getSpawnPosition(int id)
    {
        return spawnPositions[id];
    }
}
