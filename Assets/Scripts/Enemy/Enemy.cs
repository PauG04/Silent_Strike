using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class Enemy : Character
{
    public enum enemyState { GENERATING, RUNNING, CHARGING, ATTACK, RECOVERY, HURT, DIE };
    public enemyState currentState;
    public enemyState hurtLastState;

    [Header("MovementEnemy")]
    [SerializeField] private Vector3 separationDistance;
    [SerializeField] protected float speedDivider;
    [SerializeField] protected float current_speed;
    protected GameObject target;
    protected Vector3 direction;

    [Header("Forces")]
    [SerializeField] private float cohesionWeight;
    [SerializeField] private float separationWeight;
    [SerializeField] private float alligmentWeight;
    [SerializeField] private float knockBackForce;
    private Vector3 separationForce;

    [Header("Recovery")]
    [SerializeField] private float recoveryTime;
    private float currentRecoveryTime;

    [Header("Collider")]
    [SerializeField] private GameObject colliderPosition;
    [SerializeField] private GameObject damageColliderPosition;

    [Header("GenerateTime")]
    [SerializeField] private float generateTime;
    private float currentGenerateTime;

    [Header("WanderRadius")]
    [SerializeField] private float wanderRadius;
    private bool generateRadius;
    private Vector3 centerPosition;
    private bool wander;

    [Header("Material")]
    [SerializeField] private Material materialShader;

    [Header("Attack")]
    [SerializeField] private string animationName;
    [SerializeField] private GameObject blood;
    private bool attackHitted;
    private bool canAttack;

    private EnemyHpSlider hpSlider;

    private void Awake()
    {
        hpSlider = GetComponent<EnemyHpSlider>();
    }

    protected void InitializeEnemy()
    {
        base.InitializeCharacter();

        currentRecoveryTime = 0;
        currentGenerateTime = 0;
        current_speed = speed;

        if (transform.position.x > target.transform.position.x)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            colliderPosition.transform.localPosition = new Vector3(-GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
            damageColliderPosition.transform.localPosition = new Vector3(-GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;
            colliderPosition.transform.localPosition = new Vector3(GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
            damageColliderPosition.transform.localPosition = new Vector3(GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        }

        generateRadius = false;
        attackHitted = false;
        canAttack = false;
        wander = false;

        Material newMaterial = new Material(materialShader);
        GetComponent<SpriteRenderer>().material = newMaterial;
    }

    protected void UpdateEnemy()
    {
        base.UpdateCharacter();
        SetOrdingLayer();
    }

    protected void Generating()
    {
        currentGenerateTime += Time.deltaTime;
        if(currentGenerateTime > generateTime)
        {
            currentState = enemyState.RUNNING;
            animator.SetBool("Running", true);
        }
    }

    private void SetOrdingLayer()
    {
        if(target != null)
        {
            if (transform.position.z > target.transform.position.z)
                GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder - 1;
            else
                GetComponent<SpriteRenderer>().sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder + 1;
        }
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

    protected void WanderTarget()
    {
        if (target != null)
        {
            WanderActivation();

            if (wander)
            {
                Wander();
                GenerateRadiusEnemy();
            }
            else
            {
                Seek();
            }
            WanderRotation();
            CalculateForces();
            MoveEnemy();
        }
    }

    private void WanderActivation()
    {
        if (target.GetComponent<Rigidbody>().velocity.magnitude > rgbd.velocity.magnitude)
        {
            wander = false;
            return;
        }
        if (Vector3.Distance(target.transform.position, transform.position) + (wanderRadius * 2) < 1)
        {
            wander = true;
        }

    }

    protected void Wander()
    {
        if(Vector3.Distance(centerPosition, transform.position) > wanderRadius)
        {
            GenerateWanderPosition();
        }
    }

    public void GenerateWanderPosition()
    {
        Vector2 randomPosition = Random.insideUnitCircle * wanderRadius;
        direction = (centerPosition + new Vector3(randomPosition.x, 0, randomPosition.y)) - transform.position;
    }

    public void GenerateRadiusEnemy()
    {
        if(!generateRadius)
        {
            if (transform.position.x > target.transform.position.x + GetComponent<SpriteRenderer>().bounds.size.x / 2 ||
            transform.position.x < target.transform.position.x - GetComponent<SpriteRenderer>().bounds.size.x / 2)
            {
                if (GetComponent<SpriteRenderer>().flipX)
                {
                    centerPosition = new Vector3(transform.position.x + GetComponent<SpriteRenderer>().bounds.size.x / 2 + wanderRadius, transform.position.y, transform.position.z);
                }
                else
                {
                    centerPosition = new Vector3(transform.position.x - GetComponent<SpriteRenderer>().bounds.size.x / 2 - wanderRadius, transform.position.y, transform.position.z);
                }
            }
            else
            {
                if (transform.position.z > target.transform.position.z)
                {
                    centerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + GetComponent<SpriteRenderer>().bounds.size.x / 2 + wanderRadius);
                }
                else
                {
                    centerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - GetComponent<SpriteRenderer>().bounds.size.x / 2 - wanderRadius);
                }
            }
            current_speed /= speedDivider;
            generateRadius = true;
            GenerateWanderPosition();
        }
    }

    protected void Seek()
    {
        if (transform.position.x > target.transform.position.x + GetComponent<SpriteRenderer>().bounds.size.x / 2 ||
            transform.position.x < target.transform.position.x - GetComponent<SpriteRenderer>().bounds.size.x / 2)
        {
            Vector3 targetPosition = Vector3.zero;

            if (GetComponent<SpriteRenderer>().flipX)
                targetPosition = new Vector3(target.transform.position.x + target.GetComponent<SpriteRenderer>().bounds.size.x / 2, transform.position.y, target.transform.position.z);
            else
                targetPosition = new Vector3(target.transform.position.x - target.GetComponent<SpriteRenderer>().bounds.size.x / 2, transform.position.y, target.transform.position.z);

            direction = targetPosition - transform.position;
        }
        else
        {
            direction = target.transform.position - transform.position;
        }

        if(generateRadius)
        {
            current_speed = speed;
            generateRadius = false;
        }


    }

    private void WanderRotation()
    {
        if (transform.position.x > target.transform.position.x && !GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if(transform.position.x <= target.transform.position.x && GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    private void Rotate()
    {
        if (rgbd.velocity.x > 0 && GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            colliderPosition.transform.localPosition = new Vector3(GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
            damageColliderPosition.transform.localPosition = new Vector3(GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        }
        else if(rgbd.velocity.x <= 0 && !GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            colliderPosition.transform.localPosition = new Vector3(-GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
            damageColliderPosition.transform.localPosition = new Vector3(-GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        }
    }

    private void MoveEnemy()
    {
        animator.SetBool("Running", true);
        Vector3 combinedDirection = (direction.normalized + separationForce).normalized;
        Vector3 movement = combinedDirection * current_speed * Time.deltaTime;
        rgbd.AddForce(movement, ForceMode.Force);
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
    protected void ChargingAttack()
    {
        animator.SetBool("Charging", true);
        rgbd.velocity = Vector3.zero;
    }

    protected void AttackReady()
    {
        currentState = enemyState.ATTACK;
        animator.SetBool("Attack", true);
        animator.SetBool("Charging", false);
    }

    protected void Attack()
    {
        animator.SetBool("Attack", false);
        currentState = enemyState.RECOVERY;
        GenerateRadiusEnemy();
    }

    protected void ChangeToAttackColor()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            GetComponent<SpriteRenderer>().material.SetColor("_SpriteColor", Color.Lerp(GetComponent<SpriteRenderer>().color, Color.red, animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1));

    }

    protected void Recovery()
    {
        currentRecoveryTime += Time.deltaTime;
        if(currentRecoveryTime >= recoveryTime)
        {
            currentState = enemyState.RUNNING;
            current_speed = speed; 
            currentRecoveryTime = 0;
            attackHitted = false;
            canAttack = false;
        }
    }
    #endregion

    //Damage
    #region
    protected void Hurt()
    {
        if (hurtLastState != enemyState.RUNNING && hurtLastState != enemyState.HURT)
            currentState = enemyState.RECOVERY;
        else
            currentState = enemyState.RUNNING;
        animator.SetBool("Hurt", false);
    }

    protected void Die()
    {
        Destroy(transform.GetChild(0).gameObject.GetComponent<BoxCollider>());
        Destroy(GetComponent<BoxCollider2D>());
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    #endregion

    public virtual void ReceiveDamageEnemy(float amount)
    {
        base.ReceiveDamage(amount);
        PrepareReceiveDamage();
        GenerateBlood();

        if (currentHP <= 0)
        {
            currentState = enemyState.DIE;
            animator.SetBool("Hurt", false);
            animator.SetBool("Die", true);
            EnemyManager.instance.DeleteEnemy(this.gameObject);
        }
        else
        {
            hurtLastState = currentState;
            currentState = enemyState.HURT;
            rgbd.AddForce(-(target.transform.position - transform.position).normalized * knockBackForce, ForceMode.Impulse);
            animator.SetBool("Hurt", true);
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);
        }
    }

    private void PrepareReceiveDamage()
    {
        rgbd.velocity = Vector3.zero;
        animator.SetBool("Running", false);
        animator.SetBool("Attack", false);
        animator.SetBool("Charging", false);

        hpSlider.UpdateSlider();
    }

    private void GenerateBlood()
    {
        GameObject _blood = Instantiate(blood);
        _blood.transform.position = transform.position;
        _blood.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
        _blood.GetComponent<Rigidbody>().AddForce(-(target.transform.position - transform.position).normalized * knockBackForce * 6, ForceMode.Impulse);
    }

    public void ChangeState(enemyState state)
    {
        currentState = state;
    }

    public float GetDamage()
    {
        return damage;
    }

    public enemyState GetCurrentState()
    {
        return currentState;
    }

    public void SetTarget(GameObject _target)
    {
        target = _target;
    }

    public void SetAttackHitted(bool state)
    {
        attackHitted = state;
    }

    public void SetCanAttack()
    {
        canAttack = true;
        GetComponent<SpriteRenderer>().material.SetColor("_SpriteColor", Color.white);
    }

    public bool GetAttackHitted()
    {
        return attackHitted;
    }

    public bool GetCanAttack()
    {
        return canAttack;
    }
}


