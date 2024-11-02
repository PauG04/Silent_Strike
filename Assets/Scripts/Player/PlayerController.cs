using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed;
    private Vector2 inputMovementDirection;
    private Vector3 movementDirection;

    [Header("HP")]
    [SerializeField] private float hp;
    private float currentHp;

    [Header("Dash")]
    [SerializeField] private float dashForce;
    private Vector3 dashDirection;

    

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        currentHp = hp;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if(Input.GetKeyDown(KeyCode.L)) { ReceiveDamage(20); }
    }

    private void Move()
    {
        movementDirection = new Vector3(inputMovementDirection.x, 0, inputMovementDirection.y);
        rb.AddForce(movementDirection * speed * Time.deltaTime, ForceMode.Force);
    }

    private void ReceiveDamage(float damage)
    {
        currentHp -= damage;
        CheckIfDead();
        Debug.Log(currentHp);
    }

    private void CheckIfDead()
    {
        if(currentHp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    public void MovementAction(InputAction.CallbackContext obj)
    {
        inputMovementDirection = obj.action.ReadValue<Vector2>();
    }

    public void DashAction(InputAction.CallbackContext obj)
    {
        if(inputMovementDirection != Vector2.zero)
            dashDirection = new Vector3(inputMovementDirection.x, 0, inputMovementDirection.y);

        rb.AddForce(dashDirection * dashForce * Time.deltaTime, ForceMode.Impulse);
    }
}
