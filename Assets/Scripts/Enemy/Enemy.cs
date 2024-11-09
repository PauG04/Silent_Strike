using UnityEngine;

public class Enemy : Character
{
    public enum enemyState { GENERATING, RUNNING, CHARGING, ATTACK, RECOVERY, HURT, DIE, BLOCK, IDLE };
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
    [SerializeField] protected GameObject colliderPosition;
    [SerializeField] protected GameObject damageColliderPosition;

    [Header("GenerateTime")]
    [SerializeField] private float generateTime;
    private float currentGenerateTime;

    private bool orbitate;
    private bool orbiteLeft;
    private float current_angle;

    [Header("Material")]
    [SerializeField] private Material materialShader;

    [Header("Attack")]
    [SerializeField] private string animationName;
    [SerializeField] private GameObject blood;
    [SerializeField] private float dashAttackForce;
    private bool attackHitted;
    private bool canAttack;

    private EnemyHpSlider hpSlider;
    protected SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject damageGameObject;
    private ShowDamage showDamage;

    private void Awake()
    {
        hpSlider = GetComponent<EnemyHpSlider>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        showDamage = damageGameObject.GetComponent<ShowDamage>();
    }

    protected void InitializeEnemy()
    {
        base.InitializeCharacter();
        InitVariables();
        InitRotation();
        CreateMaterial();
    }

    private void InitVariables()
    {
        currentRecoveryTime = 0;
        currentGenerateTime = 0;
        current_speed = speed;

        attackHitted = false;
        canAttack = false;

        animator.SetBool("Idle", true);
        currentState = enemyState.GENERATING;
    }
    private void InitRotation()
    {
        if (transform.position.x > target.transform.position.x)
        {
            spriteRenderer.flipX = true;
            colliderPosition.transform.localPosition = new Vector3(colliderPosition.transform.localPosition.x * -1, 0, 0);
            damageColliderPosition.transform.localPosition = new Vector3(damageColliderPosition.transform.localPosition.x * -1, 0, 0);
        }
        else
        {
            spriteRenderer.flipX = false;
            colliderPosition.transform.localPosition = new Vector3(colliderPosition.transform.localPosition.x, 0, 0);
            damageColliderPosition.transform.localPosition = new Vector3(damageColliderPosition.transform.localPosition.x, 0, 0);
        }
    }
    private void CreateMaterial()
    {
        Material newMaterial = new Material(materialShader);
        spriteRenderer.material = newMaterial;
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
            ChangeState(enemyState.RUNNING);
        }
    }

    private void SetOrdingLayer()
    {
        if(target != null)
        {
            if (transform.position.z > target.transform.position.z)
                spriteRenderer.sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder - 1;
            else
                spriteRenderer.sortingOrder = target.GetComponent<SpriteRenderer>().sortingOrder + 1;
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
            StartOrbitate();
            if (orbitate)
            {
                Orbite();
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

    private void StartOrbitate()
    {
        if(Vector3.Distance(target.transform.position, transform.position) > 1.2)
        {
            current_speed = speed;
            orbitate = false;
        }
        else if (Vector3.Distance(target.transform.position, transform.position) > 0.6)
        {
            if (orbitate)
                return;
            current_speed = speed / speedDivider;
        }
        else
        {
            if (orbitate)
                return;
            Vector3 relativePos = transform.position - target.transform.position;
            current_angle = Mathf.Atan2(relativePos.z, relativePos.x);
            orbitate = true;

            if (Random.Range(1, 3) == 1)
                orbiteLeft = true;
            else
                orbiteLeft = false;
        }
    }

    private void Orbite()
    {
        if(orbiteLeft)
            current_angle -= Time.deltaTime / 2.5f;
        else
            current_angle += Time.deltaTime / 2.5f;

        float xPosition = target.transform.position.x + 0.6f * Mathf.Cos(current_angle);
        float zPosition = target.transform.position.z + 0.6f * Mathf.Sin(current_angle);
        Vector3 orbitePosition = new Vector3(xPosition, 0, zPosition);
        direction = orbitePosition - transform.position;
    }

    protected void Seek()
    {
        if (transform.position.x > target.transform.position.x + target.GetComponent<SpriteRenderer>().bounds.size.x / 2 ||
            transform.position.x < target.transform.position.x - target.GetComponent<SpriteRenderer>().bounds.size.x / 2)
        {
            Vector3 targetPosition = Vector3.zero;

            if (GetComponent<SpriteRenderer>().flipX)
            {
                targetPosition = new Vector3(target.transform.position.x + target.GetComponent<SpriteRenderer>().bounds.size.x / 2, transform.position.y, target.transform.position.z);
            }      
            else
            {
                targetPosition = new Vector3(target.transform.position.x - target.GetComponent<SpriteRenderer>().bounds.size.x / 2, transform.position.y, target.transform.position.z);
            }

            direction = targetPosition - transform.position;
        }
        else
        {
            direction = target.transform.position - transform.position;
        }
    }

    private void WanderRotation()
    {
        if (transform.position.x > target.transform.position.x && !GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            colliderPosition.transform.localPosition = new Vector3(colliderPosition.transform.localPosition.x * -1, 0, 0);
            damageColliderPosition.transform.localPosition = new Vector3(damageColliderPosition.transform.localPosition.x * -1, 0, 0);
        }
        else if(transform.position.x <= target.transform.position.x && GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            colliderPosition.transform.localPosition = new Vector3(colliderPosition.transform.localPosition.x * -1, 0, 0);
            damageColliderPosition.transform.localPosition = new Vector3(damageColliderPosition.transform.localPosition.x * -1, 0, 0);
        }
    }

    private void Rotate()
    {
        if (rgbd.velocity.x > 0 && GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            colliderPosition.transform.localPosition = new Vector3(colliderPosition.transform.localPosition.x * -1, 0, 0);
            damageColliderPosition.transform.localPosition = new Vector3(damageColliderPosition.transform.localPosition.x * -1, 0, 0);
        }
        else if(rgbd.velocity.x <= 0 && !GetComponent<SpriteRenderer>().flipX)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            colliderPosition.transform.localPosition = new Vector3(colliderPosition.transform.localPosition.x * -1, 0, 0);
            damageColliderPosition.transform.localPosition = new Vector3(damageColliderPosition.transform.localPosition.x * -1, 0, 0);
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

    protected virtual void AttackReady()
    {
        ChangeState(enemyState.ATTACK);
    }

    protected virtual void Attack()
    {
        ChangeState(enemyState.RECOVERY);
    }

    protected virtual void ChangeToAttackColor()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).IsName(animationName))
            GetComponent<SpriteRenderer>().material.SetColor("_SpriteColor", Color.Lerp(GetComponent<SpriteRenderer>().color, Color.red, animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1));

    }

    protected void Recovery()
    {
        currentRecoveryTime += Time.deltaTime;
        if(currentRecoveryTime >= recoveryTime)
        {
            current_speed = speed; 
            currentRecoveryTime = 0;
            attackHitted = false;
            canAttack = false;
            ChangeState(enemyState.RUNNING);
        }
    }

    private void DashAttack()
    {
        rgbd.AddForce(direction.normalized * dashAttackForce, ForceMode.Impulse);
    }
    #endregion

    //Damage
    #region
    protected void Hurt()
    {
        if (hurtLastState != enemyState.RUNNING && hurtLastState != enemyState.HURT)
           ChangeState(enemyState.RECOVERY);
        else
            ChangeState(enemyState.RUNNING);
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

    public virtual void ReceiveDamageEnemy(float amount, bool isFlipped)
    {
        float damageReceived = amount;

        if(isFlipped == spriteRenderer.flipX)
            damageReceived = amount * 2;
        else
            damageReceived = amount;

        base.ReceiveDamage(damageReceived);

        PrepareReceiveDamage();
        GenerateBlood();
        ShowDamageText(damageReceived);

        if (currentHP <= 0)
        {
            ChangeState(enemyState.DIE);
            EnemyManager.instance.DeleteEnemy(this.gameObject);
        }
        else
        {
            hurtLastState = currentState;
            ChangeState(enemyState.HURT);
            rgbd.AddForce(-(target.transform.position - transform.position).normalized * knockBackForce, ForceMode.Impulse);
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);
        }
    }

    public void PrepareReceiveDamage()
    {
        rgbd.velocity = Vector3.zero;
        GetComponent<SpriteRenderer>().material.SetColor("_SpriteColor", Color.white);
        hpSlider.UpdateSlider();

    }

    private void GenerateBlood()
    {
        GameObject _blood = Instantiate(blood);
        _blood.transform.position = transform.position;
        _blood.GetComponent<Rigidbody>().AddForce(-(target.transform.position - transform.position).normalized * knockBackForce * 6, ForceMode.Impulse);
    }

    private void ShowDamageText(float damage)
    {
        damageGameObject.SetActive(true);
        showDamage.SetText(damage);
    }

    public void ChangeState(enemyState state)
    {
        switch (currentState)
        {
            case enemyState.GENERATING:
                break;
            case enemyState.RUNNING:
                animator.SetBool("Running", false);
                break;
            case enemyState.CHARGING:
                animator.SetBool("Charging", false);
                break;
            case enemyState.ATTACK:
                animator.SetBool("Attack", false);
                break;
            case enemyState.RECOVERY:
                animator.SetBool("Running", false);
                break;
            case enemyState.HURT:
                animator.SetBool("Hurt", false);
                break;
            case enemyState.DIE:
                break;
            case enemyState.BLOCK:
                animator.SetBool("Parry", false);
                break;
        }

        switch (state)
        {
            case enemyState.GENERATING:
                break;
            case enemyState.RUNNING:
                animator.SetBool("Running", true);
                break;
            case enemyState.CHARGING:
                animator.SetBool("Charging", true);
                rgbd.velocity = Vector3.zero;
                break;
            case enemyState.ATTACK:
                animator.SetBool("Attack", true);
                break;
            case enemyState.RECOVERY:
                animator.SetBool("Running", true);
                break;
            case enemyState.HURT:
                animator.SetBool("Hurt", true);
                break;
            case enemyState.DIE:
                animator.SetBool("Die", true);
                break;
            case enemyState.BLOCK:
                animator.SetBool("Parry", true);
                break;
        }
        currentState = state;
    }

    public virtual void PlayerDeath()
    {
        animator.SetBool("Parry", false);
        animator.SetBool("Die", false);
        animator.SetBool("Hurt", false);
        animator.SetBool("Running", false);
        animator.SetBool("Attack", false);
        animator.SetBool("Charging", false);
        colliderPosition.SetActive(false);
        damageColliderPosition.SetActive(false);
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
        SetAttackHitted(false);
        DashAttack();
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


