using UnityEngine;
using UnityEngine.InputSystem;

public class AttackPlayer : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private ParticleSystem[] slashParticles;
    [SerializeField] private float knockBackForce;
    [SerializeField] private float enemyParryKnockBackForce;
    [SerializeField] private float enemyBlockKnockBackForce;
    [SerializeField] private float playerBlockKnockBackForce;
    [SerializeField] private GameObject blood;
    [SerializeField] private float lowFrequency;
    [SerializeField] private float highFrequency;
    [SerializeField] private float duration;
    [SerializeField] private GameObject sparks;

    private Gamepad gamepad;

    private void Start()
    {
        gamepad = Gamepad.current; 
    }

    private void GenerateBlood(GameObject player)
    {
        GameObject _blood = Instantiate(blood);
        _blood.transform.position = player.transform.position;
        _blood.GetComponent<Rigidbody>().AddForce((player.transform.position - gameObject.transform.parent.transform.position).normalized * knockBackForce, ForceMode.Impulse);
    }

    public void Vibrate()
    {
        if (gamepad != null)
        {
            Invoke(nameof(StopVibration), duration);
            gamepad.SetMotorSpeeds(lowFrequency, highFrequency);
        }
    }

    private void StopVibration()    
    {
        if (gamepad != null)
        {
            gamepad.SetMotorSpeeds(0, 0);
        }

    }

    private void PlayerHit(Collider other)
    {
        enemy.SetAttackHitted(true);
        slashParticles[0].gameObject.transform.parent.transform.position = other.transform.position;
        Vibrate();
        for (int i = 0; i < slashParticles.Length; i++)
        {
            slashParticles[i].Play();
        }
        other.GetComponent<PlayerController>().ReceiveDamage(enemy.GetDamage());
        other.GetComponent<Rigidbody>().AddForce((other.transform.position - gameObject.transform.parent.transform.position).normalized * knockBackForce, ForceMode.Impulse);
        GenerateBlood(other.gameObject);
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && enemy.GetCurrentState() == Enemy.enemyState.ATTACK && !enemy.GetAttackHitted() && enemy.GetCanAttack())
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player.GetState() == PlayerController.State.DASHING || player.GetState() == PlayerController.State.DEATH)
                return;
            if (player.GetState() == PlayerController.State.PARRY && 
                player.gameObject.GetComponent<SpriteRenderer>().flipX != enemy.gameObject.GetComponent<SpriteRenderer>().flipX)
            {
                if(player.GetCanParry())
                {
                    Parry(player, other);
                    return;
                }
                else
                {
                    Block(player, other);
                    return;
                }

            }


            PlayerHit(other);
        }
    }

    private void Parry(PlayerController player, Collider other)
    {
        player.ActiveStopParry();
        GetComponentInParent<Rigidbody>().AddForce((gameObject.transform.parent.transform.position - other.transform.position).normalized * enemyParryKnockBackForce, ForceMode.Impulse);
        GetComponentInParent<Enemy>().ChangeState(Enemy.enemyState.PARRIED);
        enemy.Parry();
        CreateSpark(player, 1);
        player.SetCurrentStamina(100);
    }

    private void Block(PlayerController player, Collider other)
    {
        player.ActiveStopParry();
        player.gameObject.GetComponent<Rigidbody>().AddForce((other.transform.position - gameObject.transform.parent.transform.position).normalized * playerBlockKnockBackForce, ForceMode.Impulse);
        GetComponentInParent<Rigidbody>().AddForce((gameObject.transform.parent.transform.position - other.transform.position).normalized * enemyBlockKnockBackForce, ForceMode.Impulse);
        GetComponentInParent<Enemy>().ChangeState(Enemy.enemyState.PARRIED);
        enemy.Block();
        CreateSpark(player, 0.5f);
        player.ConsumeStamina(enemy.GetDamage() * 5);
        if (player.GetCurrentStamina() <= 0)
        {
            player.SetCurrentStamina(0);
            player.ChangeState(PlayerController.State.STUNNED);
        }
    }

    private void CreateSpark(PlayerController player, float Scale)
    {
        GameObject _sparks = Instantiate(sparks);
        _sparks.transform.SetParent(player.gameObject.transform, true);
        _sparks.transform.localScale = new Vector3(Scale, Scale, Scale);
        if (!player.gameObject.GetComponent<SpriteRenderer>().flipX)
        {
            _sparks.transform.localPosition = new Vector3(0.1f, 0.06f, 0);
            _sparks.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            _sparks.transform.localPosition = new Vector3(-0.1f, 0.06f, 0);
            _sparks.GetComponent<SpriteRenderer>().flipX = false;
        }


    }


}
