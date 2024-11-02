using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && enemy.GetCurrentState() == Enemy.enemyState.ATTACK && !enemy.GetAttackHitted())
        {
            Debug.Log(enemy.GetDamage());
            enemy.SetAttackHitted(true);
        }
    }
}
