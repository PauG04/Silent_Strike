using UnityEngine;

public class Kunai : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed;

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
            other.gameObject.GetComponent<Enemy>().ActivateKunai();
            PlayerController.instance.SetHasThrowedKunai(false);
            Destroy(this.gameObject);
        }

    }
}
