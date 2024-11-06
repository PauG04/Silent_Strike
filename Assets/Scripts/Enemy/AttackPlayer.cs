using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private ParticleSystem[] slashParticles;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && enemy.GetCurrentState() == Enemy.enemyState.ATTACK && !enemy.GetAttackHitted() && enemy.GetCanAttack())
        {
            enemy.SetAttackHitted(true);
            for(int i = 0; i<slashParticles.Length; i++) 
            {
                slashParticles[i].Play();
            }
            Debug.Log("daño");
        }
    }
}
