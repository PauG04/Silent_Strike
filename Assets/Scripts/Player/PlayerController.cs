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

    [Header("Dash")]
    [SerializeField] private float dashForce;
    [SerializeField] private string dashAnimationName;
    private bool canDash;
    private Vector3 dashDirection;

    [Header("Art")]
    [SerializeField] private Animator animator;
    private SpriteRenderer sp;

    public enum State { IDLE, RUNNING, DASHING, HURT, DEATH, ATTACKING}

    private State currentState;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        sp = GetComponent<SpriteRenderer>();
        currentHp = hp;
        canDash = true;
        dashDirection = new Vector3(1, 0, 0);
    }


    // Start is called before the first frame update
    void Start()
    {

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
                CheckIfDashFinished();
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

        if(Input.GetKeyDown(KeyCode.L)) { ReceiveDamage(20); }
    }

    #region Movement

    public void MovementAction(InputAction.CallbackContext obj)
    {
        lastInputMovementDirection = inputMovementDirection;

        inputMovementDirection = obj.action.ReadValue<Vector2>();
    }

    private void Move()
    {
        movementDirection = new Vector3(inputMovementDirection.x, 0, inputMovementDirection.y);
        rb.AddForce(movementDirection * speed * Time.deltaTime, ForceMode.Force);
    }

    public void DashAction(InputAction.CallbackContext obj)
    {
        ChangeState(State.DASHING);
    }

    private void Dash()
    {
        if (inputMovementDirection == Vector2.zero)
            dashDirection = new Vector3(lastInputMovementDirection.x, 0, lastInputMovementDirection.y);
        else
            dashDirection = new Vector3(inputMovementDirection.x, 0, inputMovementDirection.y);


        rb.AddForce(dashDirection * dashForce * Time.deltaTime, ForceMode.Impulse);
        Debug.Log(dashDirection);
        canDash = false;
    }

    private void CheckIfDashFinished()
    {
        float animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (animationTime >= 1.0f && animator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("DASH"))
        {
            if (inputMovementDirection != Vector2.zero)
                ChangeState(State.RUNNING);
            else
                ChangeState(State.IDLE);
            
            Debug.Log("Dash terminado");
            canDash = true;
        }
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
            sp.flipX = true;
        else if (inputMovementDirection.x > 0 && sp.flipX)
            sp.flipX = false;
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
                break;
            case State.DEATH:
                break;
            case State.ATTACKING:
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
                rb.velocity = Vector3.zero;
                animator.SetBool("dashing", true);
                Dash();
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

        currentState = state;
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
}
