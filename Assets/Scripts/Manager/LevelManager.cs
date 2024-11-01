using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    private bool canChangeLevel;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;

        canChangeLevel = false;
    }

    private void Update()
    {
        if(canChangeLevel && EnemyManager.instance.GetSpawnedEnemies().Count == 0)
        {

        }
    }

    public void SetCanCangeLevel(bool state)
    {
        canChangeLevel = state;
    }
}
