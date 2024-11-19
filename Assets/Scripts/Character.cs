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

    [Header("Sounds")]
    [SerializeField] protected AudioClip receiveDamageSound;
    [SerializeField] protected AudioClip attackSound;
    [SerializeField] protected AudioClip fallSound;


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
        AudioManager.instance.Play2dOneShotSound(receiveDamageSound, "Sfx");
    }

    public virtual float GetMaxHp()
    {
        return maxHP;
    }

    public virtual float GetCurrentHp()
    {
        return currentHP;
    }
}
