using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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
    [SerializeField] private float damage;
    private SpriteRenderer sp;

    [Header("Attack")]
    [SerializeField] GameObject attackCollider;
    [SerializeField] private float attackDashForceIdle;
    [SerializeField] private float attackDashForceRunning;

    [Header("Material")]
    [SerializeField] private Material materialShader;

    [Header("Slider")]
    [SerializeField] private SliderBar hpBar;
    [SerializeField] private SliderBar staminaBar;

    [Header("PostProcessing")]
    [SerializeField] private PostProcessingLerpColor postProcessingLerpColor;


    public enum State { IDLE, RUNNING, DASHING, HURT, DEATH, ATTACKING}

    private State currentState;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sp = GetComponent<SpriteRenderer>();
        currentHp = hp;
        lastInputMovementDirection = Vector3.right;
        dashDirection = Vector3.right;
        currentStamina = maxStamina;

        recoverCoroutineisRunning = false;

        hpBar.SetMaxValue(hp);
        hpBar.SetCurrentValue(currentHp);
        staminaBar.SetMaxValue(maxStamina);
        staminaBar.SetCurrentValue(currentStamina);

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

        if(Input.GetKeyDown(KeyCode.L)) { ReceiveDamage(20); }
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
        attackCollider.gameObject.SetActive(true);
        CheckIfFlipSriteRender();

        float _dashForce = 0;

        if(currentState == State.IDLE)
        {
            _dashForce = attackDashForceIdle;
        }
        else if(currentState == State.RUNNING)
        {
            _dashForce = attackDashForceRunning;
        }
        Dash(_dashForce);

        ConsumeStamina(attackStaminaConsume);
    }

    public void EndAttack()
    {
        attackCollider.gameObject.SetActive(false);

        ChangeState(State.IDLE);
    }

    public void AttackAction(InputAction.CallbackContext obj)
    {
        if (currentState == State.ATTACKING || currentState == State.DEATH)
            return;

        if (currentStamina < attackStaminaConsume)
            return;

        ChangeState(State.ATTACKING); 
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
                Attack();
                break;
            default:
                break;
        }

        currentState = state;
    }

    public float GetDamage()
    {
        return damage;
    }

    public State GetState()
    {
        return currentState;
    }
   
}
