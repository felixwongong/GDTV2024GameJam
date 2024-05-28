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
    public NetworkVariable<int> id;
    public Unit attachedUnit => _attachedUnit;
    public BaseSkill skill;

    protected override void Awake()
    {
        base.Awake();
        _attachedUnit = GetComponent<Unit>();
    }

    protected override void Start()
    {
        base.Start();
        initPlayerRpc();
    }

    [Rpc(SendTo.Server)]
    void initPlayerRpc()
    {
        id.Value = BattleController.instance.generateId;
        var spawnTransform = BattleController.instance.getSpawnPosition(id.Value);
        transform.SetPositionAndRotation(spawnTransform.position, spawnTransform.rotation);
        transform.position = spawnTransform.position;
        transform.rotation = spawnTransform.rotation;
        GoToMoveRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void GoToMoveRpc()
    {
        GoToState(PlayerStateId.Movement);
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GoToStateNoRepeat(PlayerStateId.ExecuteSkill);
        }
    }
}
