using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class BattleController : NetworkBehaviour
{
    [SerializeField] private Transform[] spawnPositions;

    private static BattleController _instance;
    public static BattleController instance => _instance;
    
    [SerializeField]
    private NetworkVariable<int> idGenerator = new();

    private Dictionary<int, Unit> _unitMap = new();
    public IReadOnlyDictionary<int, Unit> unitMap => _unitMap;

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
        return spawnPositions[id % spawnPositions.Length];
    }

    public void registerUnit(Unit unit)
    {
        _unitMap.Add(unit.id, unit);

        if (IsServer)
        {
            distributeUnitTeam();
        }
    }

    private void distributeUnitTeam()
    {
        foreach (var (_, unit) in _unitMap)
        {
            if (unit._team.Value != UnitTeam.None) continue;
            var teams = (UnitTeam[])Enum.GetValues(typeof(UnitTeam));
            foreach (var team in teams)
            {
                if (_unitMap.Values.Any(u => u._team.Value == team)) continue;
                unit._team.Value = team;
                break;
            }
        }
    }
}
