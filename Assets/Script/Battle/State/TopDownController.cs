using System.Linq;
using CofyEngine.Network;
using Unity.Netcode;
using UnityEngine;

public class TopDownController : NetworkState<PlayerState>
{
    [SerializeField] private float movespeed = 50;
    [SerializeField] private float rotateSpeed = 100;
    [SerializeField] private Transform detector;

    [Header("State")] 
    [SerializeField] private bool movable = true;

    private Camera _mainCamera;
    private Rigidbody _rb;

    [SerializeField]
    private Vector2 _input;
    private Quaternion _rotation;

    public override PlayerState id => PlayerState.Movement;
    public PlayerStateMachine psm => (PlayerStateMachine)stateMachine;
    
    protected override void StartContext()
    {
    }

    public override void _Awake()
    {
        base._Awake();
        
        _mainCamera = Camera.main;
        _rb = GetComponent<Rigidbody>();
    }

    public override void _Update(double delta)
    {
        base._Update(delta);
        
        if(!IsOwner) return;
        
        var xAxis = Input.GetAxis("Horizontal");
        var yAxis = Input.GetAxis("Vertical");

        _input =  new Vector2(xAxis, yAxis);
    }

    public override void _FixedUpdate(double fixedDelta)
    {
        base._FixedUpdate(fixedDelta);
        
        Vector3 _velocity;
        
        if (_input != Vector2.zero)
        {
            var cameraRotation = Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0);
            var inputRotation = Quaternion.LookRotation(new Vector3(_input.x, 0, _input.y), transform.up);

            _rotation = cameraRotation * inputRotation;
        }
        else
        {
            _velocity = Vector3.zero;
            updateRigidBodyRpc(_velocity, _rotation);
            return;
        }
        
        Ray ray = new Ray(detector.position, _rb.transform.forward);
        var hits = Physics.SphereCastAll(ray, 0.2f, 0.1f);
        if (hits.Any(hit => hit.collider.name != psm.attachedUnit.team.ToString() && hit.collider.name != UnitTeam.None.ToString()))
        {
            _velocity = Vector3.zero;
        }
        else
        {
            _velocity = transform.forward * (_input.magnitude * movespeed);
        }
        
        updateRigidBodyRpc(_velocity, _rotation);
    }

    [Rpc(SendTo.Server)]
    void updateRigidBodyRpc(Vector3 velocity, Quaternion rotation)
    {
        _rb.rotation = Quaternion.RotateTowards(_rb.rotation, rotation, rotateSpeed * Time.fixedDeltaTime).normalized;
        _rb.velocity = velocity;
        Debug.Log(velocity);
    }
}
