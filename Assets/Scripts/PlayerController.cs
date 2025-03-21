using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    //-------------Componentes-------------
    private CharacterController _controller;

    //-------------Inputs----------------
    [SerializeField] private float _speed = 5f;
    private float _horizontal;
    //-------------Gravedad------------------
    [SerializeField] private float _gravity = -9.81f;
    [SerializeField] private Vector3 _playerGravity;
    [SerializeField] private float _jumpHeight = 3.0f;

    //-------------GroundSensor--------------
    [SerializeField] Transform _sensorPosition;
    [SerializeField] float _sensorRadius = 0.5f;
    [SerializeField] LayerMask _groundLayer;

    private string lastKey;

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");

        if (!IsGrounded())
        {
            _speed = 2.5f;
        }
        else
        {
            _speed = 5;
        }

        if(Input.GetButtonDown("Jump") && IsGrounded())
        {
            Jump();
            if(_horizontal < 0)
            {
                lastKey = "left";
            }
            else if(_horizontal > 0)
            {
                lastKey = "right";
            }
        }    

        Movement();

        Gravity();
    }

    void Movement()
    {
        Vector3 direction = new Vector3(_horizontal, 0, 0);

        if(direction != Vector3.zero && IsGrounded())
        {
            float targetAngle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0, targetAngle, 0);

            Vector3 moveDirection = Quaternion.Euler(0, targetAngle, 0) * Vector3.right;

            _controller.Move(moveDirection * _speed * Time.deltaTime);
        }
        else if(direction != Vector3.zero && !IsGrounded())
        {
            if(lastKey == "left" && _horizontal > 0)
            {
                _speed = 2.5f;
            }
            else if(lastKey == "right" && _horizontal < 0)
            {
                _speed = 2.5f;
            }
            else
            {
                _speed = 5f;
            }

            _controller.Move(direction * _speed * Time.deltaTime);
        }

    }

    void Jump()
    {
        _playerGravity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
    }

    void Gravity()
    {
        if(!IsGrounded())
        {
            _playerGravity.y += _gravity * Time.deltaTime;
        }
        else if(IsGrounded() && _playerGravity.y < 0)
        {
            _playerGravity.y = -1;
        }
        
        _controller.Move(_playerGravity * Time.deltaTime);

    }

    bool IsGrounded()
    {
        return Physics.CheckSphere(_sensorPosition.position, _sensorRadius, _groundLayer);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_sensorPosition.position, _sensorRadius);
    }

}