using System.Collections;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Movement")]
    [SerializeField] private float speed;
    private Vector2 inputMovementDirection;
    private Vector2 lastInputMovementDirection;
    private Vector3 movementDirection;

    [Header("HP")]
    [SerializeField] private float hp;
    private float currentHp;

    [Header("Stamina")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float attackStaminaConsume;
    [SerializeField] private float timeToRecoverStamina;
    [SerializeField] private float staminaRecoverValue;
    private float currentStamina;
    private bool canRecoverStamina;
    private bool recoverCoroutineisRunning;

    [Header("Dash")]
    [SerializeField] private float dashForce;
    private Vector3 dashDirection;

    [Header("Art")]
    [SerializeField] private Animator animator;
    private SpriteRenderer sp;

    [Header("Attack")]
    [SerializeField] GameObject attackCollider;
    [SerializeField] private float attackDashForceIdle;
    [SerializeField] private float attackDashForceRunning;
    [SerializeField] private float damage;
    [SerializeField] private float timeToStopAttackCombo;
    private AttackState currentAttackState;
    private float attackCount;
    private bool isInCombo;
    private enum AttackState { NONE, FIRST_ATTACK, SECOND_ATTACK, THIRD_ATTACK,}

   
    [Header("Kunai")]
    [SerializeField] private GameObject kunai;
    [SerializeField] private float timeToDesappearKunai;
    [SerializeField] private float kunaiStaminaConsume;
    private GameObject kunaiTarget;
    private bool hasThrowedKunai;
    private bool kunaiAttack;

    [Header("Material")]
    [SerializeField] private Material materialShader;

    [Header("Slider")]
    [SerializeField] private SliderBar hpBar;
    [SerializeField] private SliderBar staminaBar;

    [Header("PostProcessing")]
    [SerializeField] private PostProcessingLerpColor postProcessingLerpColor;

    private SpriteRenderer spriteRenderer;

    public enum State { IDLE, RUNNING, DASHING, HURT, DEATH, ATTACKING, THROWING}

    private State currentState;

    private Rigidbody rb;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;

        rb = GetComponent<Rigidbody>();
        sp = GetComponent<SpriteRenderer>();
        spriteRenderer = GetComponent<SpriteRenderer>();
      
        // HP & Stamina
        currentHp = hp;
        hpBar.SetMaxValue(hp);
        hpBar.SetCurrentValue(currentHp);

        currentStamina = maxStamina;
        staminaBar.SetMaxValue(maxStamina);
        staminaBar.SetCurrentValue(currentStamina);

        //Attack
        attackCount = 0;
        isInCombo = false;
        hasThrowedKunai = false;
        kunaiAttack = false;

        //Movement
        lastInputMovementDirection = Vector3.right;
        dashDirection = Vector3.right;
        recoverCoroutineisRunning = false;

        Material newMaterial = new Material(materialShader);
        GetComponent<SpriteRenderer>().material = newMaterial;
    }


    // Start is called before the first frame update
    void Start()
    {
        attackCollider.transform.localPosition = new Vector3(GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case State.IDLE:
                CheckIfRunning();
                break;
            case State.RUNNING:
                CheckIfIdle();
                Move();
                CheckIfFlipSriteRender();
                break;
            case State.DASHING:
                break;
            case State.HURT:
                break;
            case State.DEATH:
                break;
            case State.ATTACKING:
                break;
            default:
                break;
        }

        RecoverStamina();
    }

    #region Movement

    public void MovementAction(InputAction.CallbackContext obj)
    {
        inputMovementDirection = obj.action.ReadValue<Vector2>();
       
        if(inputMovementDirection != Vector2.zero)
            lastInputMovementDirection = inputMovementDirection;

    }

    private void Move()
    {
        movementDirection = new Vector3(inputMovementDirection.x, 0, inputMovementDirection.y);
        rb.AddForce(movementDirection * speed * Time.deltaTime, ForceMode.Force);
    }

    public void DashAction(InputAction.CallbackContext obj)
    {
        if (currentState == State.DASHING || currentState == State.DEATH)
            return;

        ChangeState(State.DASHING);
    }

    private void Dash(float _dashForce)
    {
        CheckIfFlipSriteRender();

        dashDirection = new Vector3(lastInputMovementDirection.normalized.x, 0, lastInputMovementDirection.normalized.y);

        rb.AddForce(dashDirection * _dashForce, ForceMode.Impulse);
    }

    public void EndDash()
    {
        if (inputMovementDirection != Vector2.zero)
            ChangeState(State.RUNNING);
        else
            ChangeState(State.IDLE);
    }

    private void CheckIfIdle()
    {
        if (inputMovementDirection == Vector2.zero)
            ChangeState(State.IDLE);
    }

    private void CheckIfRunning()
    {
        if (inputMovementDirection != Vector2.zero)
            ChangeState(State.RUNNING);
    }

    private void CheckIfFlipSriteRender()
    {
        if (inputMovementDirection.x < 0 && !sp.flipX)
        {
            sp.flipX = true;
            attackCollider.transform.localPosition = new Vector3(-GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        }
        else if (inputMovementDirection.x > 0 && sp.flipX)
        {
            sp.flipX = false;
            attackCollider.transform.localPosition = new Vector3(GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        }
    }

    #endregion

    #region Attack

    private void Attack()
    {
        CheckIfFlipSriteRender();

        float _dashForce = 0;

        if (rb.velocity == Vector3.zero)
        {
            _dashForce = attackDashForceIdle;
        }
        else
        {
            _dashForce = attackDashForceRunning;
        }

        if(!kunaiAttack)
            Dash(_dashForce);

        ConsumeStamina(attackStaminaConsume);
    }

    private void EndFirstAttack()
    {
        if(isInCombo)
        {
            isInCombo = false;
            AttackChangeState(AttackState.SECOND_ATTACK);
            return;
        }

        isInCombo = false;
        attackCount = 0;

        ChangeState(State.IDLE);
        AttackChangeState(AttackState.NONE);
    }

    private void EndSecondAttack()
    {
        if (isInCombo)
        {
            isInCombo = false;
            AttackChangeState(AttackState.THIRD_ATTACK);
            return;
        }

        isInCombo = false;
        attackCount = 0;

        ChangeState(State.IDLE);
        AttackChangeState(AttackState.NONE);
    }

    private void EndThirdAttack()
    {
        attackCount = 0;
        isInCombo = false;

        ChangeState(State.IDLE);
        AttackChangeState(AttackState.NONE);
    }

    public void AttackAction(InputAction.CallbackContext obj)
    {
        if (!obj.started)
            return;

        if (currentState == State.ATTACKING)
        {
            isInCombo = true;
            return;
        }
                
        if (currentState == State.DEATH)
            return;

        if (currentStamina < attackStaminaConsume)
            return;

        ChangeState(State.ATTACKING);
        AttackChangeState(AttackState.FIRST_ATTACK);
    }
    #endregion

    #region Throw
    public void ThrowAction(InputAction.CallbackContext obj)
    {
        if (!obj.started)
            return;

        if (currentState == State.ATTACKING || currentState == State.DEATH)
            return;

        if (!hasThrowedKunai)
        {
            if (currentStamina < kunaiStaminaConsume)
                return;

            ChangeState(State.THROWING);
        }
        else if (kunaiTarget != null)
        {
            KunaiAttack();
        }
    }

    private void KunaiAttack()
    {
        //freezear enemy
        SpriteRenderer enemySpriteRenderer = kunaiTarget.GetComponent<SpriteRenderer>();
        spriteRenderer.flipX = enemySpriteRenderer.flipX;

        if(enemySpriteRenderer.flipX)
        {
            transform.position = kunaiTarget.transform.position + new Vector3(kunaiTarget.GetComponent<SpriteRenderer>().bounds.size.x / 2.5f, 0f, 0f);
            attackCollider.transform.localPosition = new Vector3(-GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        }
        else
        {
            transform.position = kunaiTarget.transform.position - new Vector3(kunaiTarget.GetComponent<SpriteRenderer>().bounds.size.x / 2.5f, 0f, 0f);
            attackCollider.transform.localPosition = new Vector3(GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        }

        lastInputMovementDirection = (kunaiTarget.transform.position - transform.position).normalized;

        kunaiTarget.GetComponent<Enemy>().ChangeState(Enemy.enemyState.FREEZED);
        SetHasThrowedKunai(false);

        kunaiAttack = true;
        kunaiTarget.GetComponent<Enemy>().DesactivateKunai();

        ChangeState(State.ATTACKING);
        AttackChangeState(AttackState.FIRST_ATTACK);
        kunaiTarget = null;
    }

    private void Throw()
    {
        hasThrowedKunai = true;
        GameObject newKunai = Instantiate(kunai, transform.position, Quaternion.identity);
        newKunai.transform.transform.right = new Vector3(lastInputMovementDirection.normalized.x, 0, lastInputMovementDirection.normalized.y);

        ConsumeStamina(kunaiStaminaConsume);
    }

    private void EndThrow()
    {
        kunaiAttack = false; 
        if (inputMovementDirection != Vector2.zero)
        {
            ChangeState(State.RUNNING);
        }

        else
            ChangeState(State.IDLE);
    }
    #endregion

    #region HP

    public void ReceiveDamage(float damage)
    {
        currentHp -= damage;
        ChangeState(State.HURT);
        hpBar.SetCurrentValue(currentHp);
        postProcessingLerpColor.ChangeVignetteColor(currentHp / hp);
    }

    public void HurtFinished()
    {
        if (inputMovementDirection != Vector2.zero)
        {
            ChangeState(State.RUNNING);
            return;
        }

        ChangeState(State.IDLE);
    }

    private void CheckIfDead()
    {
        if (currentHp <= 0)
        {
            ChangeState(State.IDLE);
            Die();
            return;
        }

        HurtFinished();
    }

    private void Die()
    {
        ChangeState(State.DEATH);
        EnemyManager.instance.SetNullTarget();
        animator.SetBool("dead", true);
    }

    #endregion

    #region Stamina

    private void ConsumeStamina(float staminaConsumeValue)
    {
        canRecoverStamina = false;
        currentStamina -= staminaConsumeValue;

        staminaBar.SetCurrentValue(currentStamina);

        if (recoverCoroutineisRunning)
        {
            StopCoroutine("CanRecoverStamina");
        }

        StartCoroutine("CanRecoverStamina");
    }

    private IEnumerator CanRecoverStamina()
    {
        recoverCoroutineisRunning = true;

        yield return new WaitForSeconds(timeToRecoverStamina);

        canRecoverStamina = true;
        recoverCoroutineisRunning = false;
    }

    private void RecoverStamina()
    {
        if (!canRecoverStamina)
            return;

        if(currentStamina <= maxStamina)
        {
            currentStamina += staminaRecoverValue;


            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }

            staminaBar.SetCurrentValue(currentStamina);
        }
    }

    #endregion

    public void ChangeState(State state)
    {
        switch (currentState)
        {
            case State.IDLE:
               
                break;
            case State.RUNNING:
                animator.SetBool("running", false);
                break;
            case State.DASHING:
                animator.SetBool("dashing", false);
                rb.velocity = Vector3.zero;
                break;
            case State.HURT:
                animator.SetBool("hurt", false);
                break;
            case State.DEATH:
                break;
            case State.ATTACKING:
                animator.SetBool("attacking", false);
                attackCollider.SetActive(true);
                break;
            case State.THROWING:
                animator.SetBool("throwing", false);
                break;
            default:
                break;
        }

        switch (state)
        {
            case State.IDLE:
                animator.SetBool("running", false);
                break;
            case State.RUNNING:
                animator.SetBool("running", true);
                break;
            case State.DASHING:
                animator.SetBool("dashing", true);
                rb.velocity = Vector3.zero;
                Dash(dashForce);
                break;
            case State.HURT:
                animator.SetBool("hurt", true);
                break;
            case State.DEATH:
                animator.SetBool("hurt", false);
                break;
            case State.ATTACKING:
                animator.SetBool("attacking", true);
                break;
            case State.THROWING:
                animator.SetBool("throwing", true);
                Throw();
                break;
            default:
                break;
        }

        currentState = state;
    }

    private void AttackChangeState(AttackState _attackState)
    {
        switch (currentAttackState)
        {
            case AttackState.NONE:
                break;
            case AttackState.FIRST_ATTACK:
                animator.SetBool("firstAttack", false);
                break;
            case AttackState.SECOND_ATTACK:
                animator.SetBool("secondAttack", false);
                break;
            case AttackState.THIRD_ATTACK:
                animator.SetBool("thirdAttack", false);
                break;
            default:
                break;
        }

        switch (_attackState)
        {
            case AttackState.NONE:
                
                break;
            case AttackState.FIRST_ATTACK:
                animator.SetBool("firstAttack", true);
                Attack();
                break;
            case AttackState.SECOND_ATTACK:
                animator.SetBool("secondAttack", true);
                Attack();
                break;
            case AttackState.THIRD_ATTACK:
                animator.SetBool("thirdAttack", true);
                Attack();
                break;  
            default:
                break;
        }

        kunaiAttack = false;
        currentAttackState = _attackState;
    }

    public float GetDamage()
    {
        return damage;
    }

    public State GetState()
    {
        return currentState;
    }
   
    public void SetKunaiTarget(GameObject target)
    {
        kunaiTarget = target;
    }

    public float GetTimeToDesappearKunai()
    {
        return timeToDesappearKunai;
    }

    public void SetHasThrowedKunai(bool _hasThrowedKunai)
    {
        hasThrowedKunai = _hasThrowedKunai;
    }
}
