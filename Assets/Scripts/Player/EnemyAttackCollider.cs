using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackCollider : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] slashParticles;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            slashParticles[0].gameObject.transform.parent.transform.position = other.transform.position;
            for (int i = 0; i < slashParticles.Length; i++)
            {
                slashParticles[i].Play();
            }
            other.GetComponent<Enemy>().ReceiveDamageEnemy(GetComponentInParent<PlayerController>().GetDamage());
            gameObject.SetActive(false);
        }
    }
}
