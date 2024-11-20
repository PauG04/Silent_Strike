using UnityEngine;

public class Kunai : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed;
    [SerializeField] private float damage;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        rb.velocity = transform.right * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            PlayerController.instance.SetKunaiTarget(other.gameObject);
            other.GetComponent<Enemy>().ReceiveDamageEnemy(damage, false);
            other.gameObject.GetComponent<Enemy>().ActivateKunai();
            Destroy(this.gameObject);
        }

        if (other.CompareTag("Wall"))
        {
            PlayerController.instance.SetHasThrowedKunai(false);
            Destroy(this.gameObject);
        }

    }
}
