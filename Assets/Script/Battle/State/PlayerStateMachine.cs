using CofyEngine;
using UnityEngine;

public enum PlayerState
{
    Movement,
    ExecuteSkill
}

public class PlayerStateMachine: MonoStateMachine<PlayerState>
{
    [SerializeField] private Unit _attachedUnit;
    public Unit attachedUnit => _attachedUnit;
    public BaseSkill skill;

    protected override void Awake()
    {
        base.Awake();
        _attachedUnit = GetComponent<Unit>();
    }

    protected override void Start()
    {
        GoToState(PlayerState.Movement);
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GoToStateNoRepeat(PlayerState.ExecuteSkill);
        }
    }
}
