using CofyEngine;
using UnityEngine;

public class TopDownController : MonoState<PlayerState>
{
    [SerializeField] private float movespeed = 50;
    [SerializeField] private float rotateSpeed = 100;

    private Camera _mainCamera;
    private Rigidbody _rb;

    private Vector2 _input;
    private Quaternion _rotation;

    public override PlayerState id => PlayerState.Movement;
    protected internal override void StartContext(MonoStateMachine<PlayerState> sm, object param)
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
        
        _rb.rotation = Quaternion.RotateTowards(_rb.rotation, _rotation, rotateSpeed * Time.fixedDeltaTime).normalized;
        _rb.velocity = transform.forward * (_input.magnitude * movespeed);
    }
}
