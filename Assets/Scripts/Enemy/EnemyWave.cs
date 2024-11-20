using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Enemy Wave", menuName ="Wave")]
public class EnemyWave : ScriptableObject
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public float spawnTime;
    }

    public List<EnemySpawnData> enemies;
}
