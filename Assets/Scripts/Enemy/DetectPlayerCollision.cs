using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectPlayerCollision : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject);
        if(other.gameObject.CompareTag("Player") && enemy.GetCurrentState() == Enemy.enemyState.RUNNING)
        {
           enemy.ChangeState(Enemy.enemyState.CHARGING);
        }
    }
}
