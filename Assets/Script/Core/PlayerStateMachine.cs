using CofyEngine;

public enum PlayerState
{
    Movement,
    Place
}

public class PlayerStateMachine: MonoStateMachine<PlayerState>
{
    void Start()
    {
        GoToState(PlayerState.Movement);
    }
}
