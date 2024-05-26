using CofyEngine;
using UnityEngine;

public enum PlayerState
{
    Movement,
    ExecuteSkill
}

public class PlayerStateMachine: MonoStateMachine<PlayerState>
{
    public BaseSkill skill;
    
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
