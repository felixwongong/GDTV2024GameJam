using System;
using System.Linq;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

public class TopDownController : PlayerState
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
    private Vector3 _velocity;
    private Quaternion _rotation = quaternion.identity;

    public override PlayerStateId id => PlayerStateId.Movement;
    public PlayerStateMachine psm => (PlayerStateMachine)stateMachine;
    
    protected override void StartContext()
    {
        Debug.Log("Start movement state");
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
        
        Debug.Log($"velocity: {_rb.velocity}, input: {_input}");
        handleInputServerRpc(_input);
    }

    [Rpc(SendTo.Server)]
    private void handleInputServerRpc(Vector2 input)
    {
        Vector3 _velocity;
        
        if (input != Vector2.zero)
        {
            var cameraRotation = Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0);
            var inputRotation = Quaternion.LookRotation(new Vector3(input.x, 0, input.y), transform.up);

            _rotation = cameraRotation * inputRotation;
        }
        else
        {
            this._velocity = Vector3.zero;
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
            _velocity = transform.forward * (input.magnitude * movespeed);
        }
        
        this._velocity = _velocity;
        Debug.Log($"isOwner: {IsOwner}, velocity: {_velocity}");
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            if(_rotation != quaternion.identity) _rb.rotation = _rotation;
            _rb.velocity = _velocity;
        }
    }
}
