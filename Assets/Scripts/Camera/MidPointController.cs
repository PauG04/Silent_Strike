using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidPointController : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = PlayerManager.instance.GetPlayer();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (playerController == null)
            return;

         transform.position = playerController.transform.position;
    }


}
