using Script.Core;
using Script.UI.Component;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public enum UnitTeam: int
{
    None = 0,
    Ocean = 1,
    Forest = 2,
    Volcano = 3,
    Last,
}

public class Unit : NetworkBehaviour
{
    [SerializeField] private TeamProperties _teamProperties;
    [SerializeField] private SkinnedMeshRenderer unitMesh;
    [SerializeField] private Light light;
    
    public int id;
    public NetworkVariable<UnitTeam> _team = new(UnitTeam.None);
    public UnitTeam team => _team.Value; 

    //State
    [SerializeField] private Vector2 _input_Axis;

    public Vector2 input_Axis => _input_Axis;
    //
    
    //UI
    public HpBarElement hpBar;
    public ProgressBarElement progressBar;
    //
    
    private PlayerStateMachine _psm;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        id = Random.Range(0, 100);
        _psm = GetComponent<PlayerStateMachine>();
        _psm.setUnit(this);

        light = GetComponentInChildren<Light>();
    }

    private void Start()
    {
        BattleController.instance.registerUnit(this);
        
        var spawnPos = BattleController.instance.getSpawnPosition(id);
        transform.position = spawnPos.position;
        transform.rotation = spawnPos.rotation;
        _psm.GoToState(PlayerStateId.Movement);
    }
    
    protected void Update()
    {
        if(!IsOwner) return;
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _psm.GoToStateNoRepeat(PlayerStateId.ExecuteSkill);
        }
        
        var xAxis = Input.GetAxis("Horizontal");
        var yAxis = Input.GetAxis("Vertical");

        _input_Axis =  new Vector2(xAxis, yAxis);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void assignTeamRpc(UnitTeam team)
    {
        this._team.Value = team;
        var property = _teamProperties.properties.Find(property => property.team == team);
        if (property != null)
        {
            unitMesh.materials[1].color = property.color;
            light.color = property.color;
        }
    }
}