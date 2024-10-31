using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class Enemy : Character
{
    public enum enemyState { IDLE, RUNNING, ATTACK, HURT, DIE };
    public enemyState currentState;

    [Header("MovementEnemy")]
    [SerializeField] private Vector3 separationDistance;
    [SerializeField] protected GameObject target;
    protected Vector3 direction;
    
    [Header("Forces")]
    [SerializeField] private float cohesionWeight;
    [SerializeField] private float separationWeight;
    [SerializeField] private float alligmentWeight;
    private Vector3 separationForce;


    protected void InitializeEnemy()
    {
        base.InitializeCharacter();
        currentState = enemyState.RUNNING;
    }

    protected void UpdateEnemy()
    {
        base.UpdateCharacter();
    }

    //Movement
    #region 
    protected void SeekTarget()
    {
        if (target != null)
        {
            Seek();
            CalculateForces();
            Rotate();
            MoveEnemy();
        }

    }

    private void Rotate()
    {
        if (rgbd.velocity.x > 0)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;
    }

    private void MoveEnemy()
    {
        Vector3 combinedDirection = (direction.normalized + separationForce).normalized;
        Vector3 movement = combinedDirection * speed * Time.deltaTime;
        rgbd.velocity = movement.normalized;
    }

    private void Seek()
    {
        Vector3 targetPosition = Vector3.zero;

        if (!GetComponent<SpriteRenderer>().flipX)
            targetPosition = new Vector3(target.transform.position.x + target.GetComponent<SpriteRenderer>().bounds.size.x / 2, transform.position.y, target.transform.position.z);
        else
            targetPosition = new Vector3(target.transform.position.x - target.GetComponent<SpriteRenderer>().bounds.size.x / 2, transform.position.y, target.transform.position.z);
        direction = targetPosition - transform.position;
    }
    #endregion

    //Forces
    #region 
    private void CalculateForces()
    {
        separationForce = Vector3.zero;
        Collider[] neighbours = GetNeighbours();

        if (neighbours.Length - 1 > 0)
        {
            CalculateSeparationFoce(neighbours);
            ApplyAlligment(neighbours);
            ApplyCohesion(neighbours);
        }
    }

    private void CalculateSeparationFoce(Collider[] neighbours)
    {
        foreach (var neighbour in neighbours)
        {
            Vector3 direction = neighbour.transform.position - transform.position;
            float distance = direction.magnitude;
            Vector3 away = -direction.normalized;

            if (distance > 0)
            {
                separationForce += away / distance * separationWeight;
            }
        }
    }

    private void ApplyAlligment(Collider[] neighbours)
    {
        Vector3 neighboursForward = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            neighboursForward += neighbour.transform.forward;
        }

        if (neighboursForward != Vector3.zero)
        {
            neighboursForward.Normalize();
        }

        separationForce += neighboursForward * alligmentWeight;
    }
    private void ApplyCohesion(Collider[] neighbours)
    {
        Vector3 avaragePosition = Vector3.zero;

        foreach (var neighbour in neighbours)
        {
            avaragePosition += neighbour.transform.position;
        }

        avaragePosition /= neighbours.Length;
        Vector3 cohesionDirection = (avaragePosition - transform.position).normalized;
        separationForce += cohesionDirection * cohesionWeight;
    }


    private Collider[] GetNeighbours()
    {
        LayerMask enemyMask = LayerMask.GetMask("Enemy");
        return Physics.OverlapBox(transform.position, separationDistance, Quaternion.identity, enemyMask);
    }
    #endregion

    //Attack
    #region
    protected void Attack()
    {
        animator.SetBool("Attack", true);
        rgbd.velocity = Vector3.zero;
    }
    #endregion

    protected virtual void ReceiveDamage(float amount)
    {
        base.ReceiveDamage(amount);
    }

    public void ChangeState(enemyState state)
    {
        currentState = state;
    }
}
