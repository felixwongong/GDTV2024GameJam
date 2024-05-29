using CofyEngine.Network;
using Unity.Netcode;
using UnityEngine;

public enum PlayerStateId
{
    Movement,
    ExecuteSkill
}

public abstract class PlayerState : NetworkState<PlayerStateId>
{
    
}

public class PlayerStateMachine: NetworkStateMachine<PlayerStateId, PlayerState>
{
    [SerializeField] private Unit _attachedUnit;
    public Unit attachedUnit => _attachedUnit;
    public BaseSkill skill;

    public void setUnit(Unit unit)
    {
        _attachedUnit = unit;
    }

}
