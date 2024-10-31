using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected float speed;
    protected Rigidbody rgbd;

    [Header("Combat")]
    [SerializeField] protected int maxHP;
    [SerializeField] protected float damage;
    protected float currentHP;

    [Header("Animation")]
    protected Animator animator;

    protected void InitializeCharacter()
    {
        rgbd = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        currentHP = maxHP;
    }

    protected virtual void UpdateCharacter()
    {

    }

    protected virtual void ReceiveDamage(float amount)
    {
        currentHP -= amount;
        if(currentHP <= 0) 
        { 
            //animation
        }
    }
}
