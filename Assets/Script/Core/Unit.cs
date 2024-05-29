using Unity.Netcode;
using UnityEngine;

public enum UnitTeam
{
    None,
    Ocean,
    Forest,
    Volcano
}

public class Unit : NetworkBehaviour 
{
    public int id;
    public UnitTeam team;

    private PlayerStateMachine _psm;
    
    private void Awake()
    {
        id = Random.Range(0, 100);
        _psm = GetComponent<PlayerStateMachine>();
    }

    private void Start()
    {
        _psm.setUnit(this);
        var spawnPos = BattleController.instance.getSpawnPosition(id);
        transform.position = spawnPos.position;
        transform.rotation = spawnPos.rotation;
        _psm.GoToState(PlayerStateId.Movement);
    }
    
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _psm.GoToStateNoRepeat(PlayerStateId.ExecuteSkill);
        }
    }
}