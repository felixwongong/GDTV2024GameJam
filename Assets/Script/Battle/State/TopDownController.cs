using System;
using System.Linq;
using CofyEngine;
using UnityEngine;

public class TopDownController : MonoState<PlayerState>
{
    [SerializeField] private float movespeed = 50;
    [SerializeField] private float rotateSpeed = 100;
    [SerializeField] private Transform detector;

    [Header("State")] 
    [SerializeField] private bool movable = true;

    private Camera _mainCamera;
    private Rigidbody _rb;

    private Vector2 _input;
    private Quaternion _rotation;

    public override PlayerState id => PlayerState.Movement;
    public PlayerStateMachine psm => (PlayerStateMachine)stateMachine;
    
    protected internal override void StartContext(object param)
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
        
        var xAxis = Input.GetAxis("Horizontal");
        var yAxis = Input.GetAxis("Vertical");

        _input =  new Vector2(xAxis, yAxis);
    }

    public override void _FixedUpdate(double fixedDelta)
    {
        base._FixedUpdate(fixedDelta);
        
        if (_input != Vector2.zero)
        {
            var cameraRotation = Quaternion.Euler(0, _mainCamera.transform.eulerAngles.y, 0);
            var inputRotation = Quaternion.LookRotation(new Vector3(_input.x, 0, _input.y), transform.up);

            _rotation = cameraRotation * inputRotation;
        }
        else
        {
            _rb.velocity = Vector3.zero;
        }
        
        _rb.rotation = Quaternion.RotateTowards(_rb.rotation, _rotation, rotateSpeed * Time.fixedDeltaTime).normalized;

        Ray ray = new Ray(detector.position, _rb.transform.forward);
        var hits = Physics.SphereCastAll(ray, 0.2f, 0.1f);
        if (hits.Any(hit => hit.collider.name != psm.attachedUnit.team.ToString() && hit.collider.name != UnitTeam.None.ToString()))
        {
            _rb.velocity = Vector3.zero;
        }
        else
        {
            _rb.velocity = transform.forward * (_input.magnitude * movespeed);
        }
    }
}
