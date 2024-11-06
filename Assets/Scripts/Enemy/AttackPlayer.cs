using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AttackPlayer : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private ParticleSystem[] slashParticles;
    [SerializeField] private float knockBackForce;
    [SerializeField] private GameObject blood;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && enemy.GetCurrentState() == Enemy.enemyState.ATTACK && !enemy.GetAttackHitted() && enemy.GetCanAttack())
        {
            enemy.SetAttackHitted(true);
            slashParticles[0].gameObject.transform.parent.transform.position = other.transform.position;
            for(int i = 0; i<slashParticles.Length; i++) 
            {
                slashParticles[i].Play();
            }
            other.GetComponent<PlayerController>().ReceiveDamage(enemy.GetDamage());
            other.GetComponent<Rigidbody>().AddForce((other.transform.position - gameObject.transform.parent.transform.position).normalized * knockBackForce, ForceMode.Impulse);
            GenerateBlood(other.gameObject);
        }
    }

    private void GenerateBlood(GameObject player)
    {
        GameObject _blood = Instantiate(blood);
        _blood.transform.position = player.transform.position;
        _blood.GetComponent<SpriteRenderer>().sortingOrder = player.GetComponent<SpriteRenderer>().sortingOrder + 1;
        _blood.GetComponent<Rigidbody>().AddForce((player.transform.position - gameObject.transform.parent.transform.position).normalized * knockBackForce * 6, ForceMode.Impulse);
    }
}
