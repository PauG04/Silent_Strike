using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }

        instance = this;
    }

    [Header("EnemiesWave")]
    [SerializeField] private EnemyWave enemyWave;
    [SerializeField] private List<Transform> spawnPosition;
    private int index;

    [Header("Enemies")]
    private List<GameObject> spawnedEnemies;

    [Header("Player")]
    [SerializeField] private GameObject target;

    [Header("Time")]
    private float currentTime;

    private void Start()
    {
        index = 0;
        spawnedEnemies = new List<GameObject>();
    }
    private void Update()
    {
        currentTime += Time.deltaTime;
        if (index < enemyWave.enemies.Count)
        {
            if (enemyWave.enemies[index].spawnTime < currentTime || (spawnedEnemies.Count == 0 && index != 0))
            {
                SpawnEnemy();
            }
        }
        else
        {
            LevelManager.instance.SetCanCangeLevel(true);
        }
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyWave.enemies[index].enemyPrefab);

        enemy.transform.position = new Vector3(spawnPosition[enemyWave.enemies[index].spawnIndex].position.x,
            spawnPosition[enemyWave.enemies[index].spawnIndex].position.y + enemy.GetComponent<SpriteRenderer>().bounds.size.y / 2,
            spawnPosition[enemyWave.enemies[index].spawnIndex].position.z);

        spawnPosition[enemyWave.enemies[index].spawnIndex].GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();
        spawnPosition[enemyWave.enemies[index].spawnIndex].GetChild(0).GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();

        spawnedEnemies.Add(enemy);
        enemy.GetComponent<Enemy>().SetTarget(target);
        index++;

    }

    public void SetNullTarget()
    {
        for(int i = 0;  i < spawnedEnemies.Count; i++) 
        {
            spawnedEnemies[i].GetComponent<Enemy>().SetTarget(null);
            spawnedEnemies[i].GetComponent<Enemy>().PlayerDeath();
        }
    }

    public List<GameObject> GetSpawnedEnemies()
    {
        return spawnedEnemies;
    }

    public void DeleteEnemy(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
    }


}
