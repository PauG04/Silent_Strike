using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerCollision : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
           enemy.ChangeState(Enemy.enemyState.ATTACK);
        }
    }
}
