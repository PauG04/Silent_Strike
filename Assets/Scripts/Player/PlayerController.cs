using System.Collections;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float movementDrag;
    private Vector2 inputMovementDirection;
    private Vector2 lastInputMovementDirection;
    private Vector3 movementDirection;

    [Header("HP")]
    [SerializeField] private float hp;
    private float currentHp;
    private bool invencibility;

    [Header("Stamina")]
    [SerializeField] private float maxStamina;
    [SerializeField] private float attackStaminaConsume;
    [SerializeField] private float timeToRecoverStamina;
    [SerializeField] private float staminaRecoverValue;
    private float currentStamina;
    private bool canRecoverStamina;
    private bool recoverCoroutineisRunning;

    [Header("Healing")]
    [SerializeField] private float healingPerUse;
    [SerializeField] private int initialHealings;
    public int currentHealings;

    [Header("Dash")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDrag;
    private Vector3 dashDirection;

    [Header("Art")]
    [SerializeField] private Animator animator;
    private SpriteRenderer sp;

    [Header("Attack")]
    [SerializeField] GameObject attackCollider;
    [SerializeField] private float attackDashForceIdle;
    [SerializeField] private float attackDashForceRunning;
    [SerializeField] private float damage;
    private AttackState currentAttackState;
    private bool isInCombo;
    private enum AttackState { NONE, FIRST_ATTACK, SECOND_ATTACK, THIRD_ATTACK, }

    [Header("Kunai")]
    [SerializeField] private GameObject kunai;
    [SerializeField] private float timeToDesappearKunai;
    [SerializeField] private float kunaiStaminaConsume;
    private GameObject kunaiTarget;
    private bool hasThrowedKunai;
    private bool kunaiAttack;

    [Header("Parry")]
    [SerializeField] private float blockSpeed;
    [SerializeField] private float parryStaminaConsume;
    [SerializeField] private float stopParryTime;
    private bool canParry;
    private bool blocking;
    private bool startConsume;
    private float initSpeed;

    [Header("Material")]
    [SerializeField] private Material materialShader;

    [Header("Slider")]
    [SerializeField] private SliderBar hpBar;
    [SerializeField] private SliderBar staminaBar;

    [Header("PostProcessing")]
    [SerializeField] private PostProcessingLerpColor postProcessingLerpColor;

    [Header("SFX")]
    [SerializeField] private AudioClip attack01Sound;
    [SerializeField] private AudioClip attack02Sound;
    [SerializeField] private AudioClip receiveDamageSound;
    [SerializeField] private AudioClip throwKunaiSound;
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip fallSound;
    [SerializeField] private AudioClip healSound;

    private SpriteRenderer spriteRenderer;

    public enum State { IDLE, RUNNING, DASHING, HURT, DEATH, ATTACKING, THROWING, PARRY, STUNNED, HEALING }

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

        // Healings
        currentHealings = initialHealings;

        //Attack
        isInCombo = false;
        hasThrowedKunai = false;
        kunaiAttack = false;

        //Movement
        lastInputMovementDirection = Vector3.right;
        dashDirection = Vector3.right;
        recoverCoroutineisRunning = false;
        movementDrag = rb.drag;

        invencibility = false;
        initSpeed = speed;
        canParry = false;
        blocking = false;
    }

    void FixedUpdate()
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
            case State.PARRY:
                Move();
                CheckIfFlipSriteRender();
                ConsumeStaminaParry();
                break;
            case State.HEALING:
                break;
            default:
                break;
        }

        RecoverStamina();

        if (Input.GetKeyDown(KeyCode.G))
            invencibility = !invencibility;

    }

    #region Movement

    public void MovementAction(InputAction.CallbackContext obj)
    {
        inputMovementDirection = obj.action.ReadValue<Vector2>();

        if (inputMovementDirection != Vector2.zero)
            lastInputMovementDirection = inputMovementDirection;

    }

    private void Move()
    {
        movementDirection = new Vector3(inputMovementDirection.x, 0, inputMovementDirection.y);
        rb.AddForce(movementDirection * speed * Time.deltaTime, ForceMode.Force);
    }

    public void DashAction(InputAction.CallbackContext obj)
    {
        if (!obj.started || currentState == State.DASHING || currentState == State.DEATH)
            return;

        ChangeState(State.DASHING);
    }

    private void Dash(float _dashForce)
    {
        CheckIfFlipSriteRender();

        dashDirection = new Vector3(lastInputMovementDirection.normalized.x, 0, lastInputMovementDirection.normalized.y);

        rb.AddForce(dashDirection * _dashForce, ForceMode.Impulse);

        AudioManager.instance.Play2dOneShotSound(dashSound, "Sfx");
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
            attackCollider.transform.localPosition = new Vector3(attackCollider.transform.localPosition.x * -1, 0, 0);
        }
        else if (inputMovementDirection.x > 0 && sp.flipX)
        {
            sp.flipX = false;
            attackCollider.transform.localPosition = new Vector3(attackCollider.transform.localPosition.x * -1, 0, 0);
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

        if (!kunaiAttack)
            Dash(_dashForce);

        ConsumeStamina(attackStaminaConsume);
    }

    private void EndFirstAttack()
    {
        if (isInCombo && CheckIfCanAct(attackStaminaConsume))
        {
            isInCombo = false;
            AttackChangeState(AttackState.SECOND_ATTACK);
            return;
        }

        isInCombo = false;

        ChangeState(State.IDLE);
        AttackChangeState(AttackState.NONE);
    }

    private void EndSecondAttack()
    {
        if (isInCombo && CheckIfCanAct(attackStaminaConsume))
        {
            isInCombo = false;
            AttackChangeState(AttackState.THIRD_ATTACK);
            return;
        }

        isInCombo = false;

        ChangeState(State.IDLE);
        AttackChangeState(AttackState.NONE);
    }

    private void EndThirdAttack()
    {
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

        if (!CheckIfCanAct(attackStaminaConsume))
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

        if (currentState != State.IDLE && currentState != State.RUNNING)
            return;

        if (!hasThrowedKunai)
        {
            if (currentStamina < kunaiStaminaConsume)
                return;

            ChangeState(State.THROWING);
        }
        else if (kunaiTarget != null)
        {
            StartCoroutine(KunaiAttack());
        }
    }

    private IEnumerator KunaiAttack()
    {
        yield return new WaitForEndOfFrame();

        //freezear enemy
        SpriteRenderer enemySpriteRenderer = kunaiTarget.GetComponent<SpriteRenderer>();

        if (enemySpriteRenderer.flipX)
        {
            transform.position = kunaiTarget.transform.position + new Vector3(enemySpriteRenderer.bounds.size.x / 2.5f, 0f, 0f);
            attackCollider.transform.localPosition = new Vector3(-enemySpriteRenderer.bounds.size.x / 2, 0, 0);
        }
        else
        {
            transform.position = kunaiTarget.transform.position - new Vector3(enemySpriteRenderer.bounds.size.x / 2.5f, 0f, 0f);
            attackCollider.transform.localPosition = new Vector3(enemySpriteRenderer.bounds.size.x / 2, 0, 0);
        }

        lastInputMovementDirection = (kunaiTarget.transform.position - transform.position).normalized;

        kunaiTarget.GetComponent<Enemy>().ChangeState(Enemy.enemyState.FREEZED);
        SetHasThrowedKunai(false);

        kunaiAttack = true;
        kunaiTarget.GetComponent<Enemy>().DesactivateKunai();

        ChangeState(State.ATTACKING);
        AttackChangeState(AttackState.FIRST_ATTACK);
        kunaiTarget = null;

        spriteRenderer.flipX = enemySpriteRenderer.flipX;
        GamepadManager.instance.Rumble(0.5f, 0.5f, 0.1f);
        //CameraShaker.instance.WeakShake(0.2f);
    }

    private void Throw()
    {
        hasThrowedKunai = true;
        GameObject newKunai = Instantiate(kunai, transform.position, Quaternion.identity);
        //newKunai.transform.transform.right = new Vector3(lastInputMovementDirection.normalized.x, 0, lastInputMovementDirection.normalized.y);
        newKunai.transform.transform.right = AutoAimKunai();

        AudioManager.instance.Play2dOneShotSound(throwKunaiSound, "Sfx");
        ConsumeStamina(kunaiStaminaConsume);
    }

    private Vector3 AutoAimKunai()
    {
        float minDistance = 10000f;
        Vector3 inputDirection = new Vector3(lastInputMovementDirection.normalized.x, 0, lastInputMovementDirection.normalized.y);
        Vector3 resultDirection = Vector3.zero;
        
        foreach (GameObject enemy in EnemyManager.instance.GetSpawnedEnemies())
        {
            Vector3 distanceToEnemy = enemy.transform.position - transform.position;
            float projection = Vector3.Dot(distanceToEnemy, inputDirection);

            if (projection > 0f && projection < minDistance)
            {
                minDistance = projection;
                resultDirection = distanceToEnemy.normalized;

            }
        }

        return resultDirection;
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

    #region Parry
    public void ParryAction(InputAction.CallbackContext obj)
    {
        if (obj.started)
        {
            ChangeState(State.PARRY);
            speed = blockSpeed;
        }
        else if (obj.canceled)
        {
            startConsume = false;
            ActiveStopParry();
        }

    }

    public void ActiveStopParry()
    {
        Invoke("StopParry", stopParryTime);
    }

    private void StopParry()
    {
        speed = initSpeed;
        canParry = false;
        blocking = false;
        if (inputMovementDirection != Vector2.zero)
            ChangeState(State.RUNNING);
        else
            ChangeState(State.IDLE);
    }

    private void ConsumeStaminaParry()
    {
        if(startConsume)
        {
            ConsumeStamina(parryStaminaConsume);
            if (currentStamina <= 0)
                StopParry();
        }
    }

    private void ActiveParry()
    {
        canParry = true;
    }

    private void DesactiveParry()
    {
        canParry = false;
        blocking = true;
        startConsume = true;
    }

    private void StartStuned()
    {
        Invoke("StopStuned", 1f);
    }

    private void StopStuned()
    {
        ChangeState(State.IDLE);
    }
    #endregion

    #region HP
    public void ReceiveDamage(float damage)
    {
        if (invencibility)
            return;

        currentHp -= damage;
        ChangeState(State.HURT);
        hpBar.SetCurrentValue(currentHp);
        postProcessingLerpColor.ChangeVignetteColor(currentHp / hp);

        GamepadManager.instance.Rumble(0.5f, 0.5f, 0.1f);
        AudioManager.instance.Play2dOneShotSound(receiveDamageSound, "Sfx");
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

    public void HealAction(InputAction.CallbackContext obj)
    {
        if (!obj.started)
            return;

        if (currentState == State.DEATH || currentState == State.HURT || currentState == State.STUNNED)
            return;

        if (currentHealings < 1 || currentHp >= hp)
            return;

        postProcessingLerpColor.ResetPostPorcessing();
        ChangeState(State.HEALING);
    }

    private void Heal()
    {
        currentHealings--;

        currentHp += healingPerUse;
        if (currentHp > hp)
            currentHp = hp;

        hpBar.SetCurrentValue(currentHp);
        AudioManager.instance.Play2dOneShotSound(healSound, "Sfx");
    }

    #endregion

    #region Stamina

    public void ConsumeStamina(float staminaConsumeValue)
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

        if (currentStamina <= maxStamina)
        {
            currentStamina += staminaRecoverValue;


            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }

            staminaBar.SetCurrentValue(currentStamina);
        }
    }

    private bool CheckIfCanAct(float value)
    {
        if (currentStamina < value)
            return false;

        return true;
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
                rb.drag = movementDrag;
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
            case State.PARRY:
                animator.SetBool("parry", false);
                break;
            case State.STUNNED:
                animator.SetBool("stunned", false);
                StartStuned();
                break;
            case State.HEALING:
                animator.SetBool("healing", false);
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
                //rb.velocity = Vector3.zero;
                rb.drag = dashDrag;
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
            case State.PARRY:
                animator.SetBool("parry", true);
                break;
            case State.STUNNED:
                animator.SetBool("stunned", true);
                break;
            case State.HEALING:
                animator.SetBool("healing", true);
                Heal();
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
                AudioManager.instance.Play2dOneShotSound(attack01Sound, "Sfx", 0.8f);
                break;
            case AttackState.SECOND_ATTACK:
                animator.SetBool("secondAttack", true);
                Attack();
                AudioManager.instance.Play2dOneShotSound(attack02Sound, "Sfx", 0.9f);
                break;
            case AttackState.THIRD_ATTACK:
                animator.SetBool("thirdAttack", true);
                Attack();
                AudioManager.instance.Play2dOneShotSound(attack01Sound, "Sfx", 0.8f);
                break;
            default:
                break;
        }

        kunaiAttack = false;
        currentAttackState = _attackState;
    }

    private void ActiveFallSound()
    {
        AudioManager.instance.Play2dOneShotSound(fallSound, "Sfx", 1.3f);
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

    public bool GetCanParry()
    {
        return canParry;
    }

    public float GetCurrentStamina()
    {
        return currentStamina;
    }

    public void SetCurrentStamina(float _stamina)
    {
        currentStamina = _stamina;
    }

    public void SetHasThrowedKunai(bool _hasThrowedKunai)
    {
        hasThrowedKunai = _hasThrowedKunai;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Wall") && currentState == State.DASHING)
            EndDash();
    }

    public float GetCurrentHealings()
    {
        return currentHealings;
    }

    private void ReturnToMainMenu()
    {
        SceneLoader.instance.ChangeScene("MainMenu");
    }
}