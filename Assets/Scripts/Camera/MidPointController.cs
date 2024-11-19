using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidPointController : MonoBehaviour
{
    [SerializeField] private float speed;

    private Vector3 posToGo;

    private void Awake()
    {
        posToGo = Vector3.zero;
    }

    void Update()
    {
        Move();
    }

 

    private void Move()
    {
        if (PlayerManager.instance.GetPlayer() == null)
            return;

         transform.position = Vector3.Lerp(transform.position, PlayerManager.instance.GetPlayer().transform.position, Time.deltaTime * speed);
    }
}
