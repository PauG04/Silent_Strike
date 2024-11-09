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
        rb.velocity = Vector3.right * speed * Time.deltaTime;
    }
}
