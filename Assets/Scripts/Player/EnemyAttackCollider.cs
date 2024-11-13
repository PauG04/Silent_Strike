using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] slashParticles;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    private void Awake()
    {
        spriteRenderer = GetComponentInParent<SpriteRenderer>();
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            slashParticles[0].gameObject.transform.parent.transform.position = other.transform.position;
            other.GetComponent<Enemy>().ReceiveDamageEnemy(playerController.GetDamage(), spriteRenderer.flipX);
            if (other.GetComponent<Enemy>().GetCurrentState() != Enemy.enemyState.BLOCK)
                GenerateParticles();
        }
    }

    private void GenerateParticles()
    {
        for (int i = 0; i < slashParticles.Length; i++)
        {
            slashParticles[i].Play();
        }
    }
}
