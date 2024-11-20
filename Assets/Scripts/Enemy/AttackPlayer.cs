using UnityEngine;
using UnityEngine.InputSystem;

public class AttackPlayer : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private ParticleSystem[] slashParticles;
    [SerializeField] private float knockBackForce;
    [SerializeField] private GameObject blood;
    [SerializeField] private float lowFrequency;
    [SerializeField] private float highFrequency;
    [SerializeField] private float duration;

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
            if (other.GetComponent<PlayerController>().GetState() == PlayerController.State.DASHING)
                return;

            PlayerHit(other);
        }
    }

   
}
