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

    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;     
        public Transform spawnPosition;
        public float spawnTime;
    }

    [Header("EnemiesType")]
    [SerializeField] private List<EnemySpawnData> enemiesToSpawn = new List<EnemySpawnData>();
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
        if (index < enemiesToSpawn.Count) 
        {
            if (enemiesToSpawn[index].spawnTime < currentTime || spawnedEnemies.Count == 0)
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
        GameObject enemy = Instantiate(enemiesToSpawn[index].enemyPrefab);

        enemy.transform.position = new Vector3(enemiesToSpawn[index].spawnPosition.position.x,
            enemiesToSpawn[index].spawnPosition.position.y + enemy.GetComponent<SpriteRenderer>().bounds.size.y / 2, 
            enemiesToSpawn[index].spawnPosition.position.z);

        enemiesToSpawn[index].spawnPosition.GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();
        enemiesToSpawn[index].spawnPosition.GetChild(0).GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();

        spawnedEnemies.Add(enemy);
        enemy.GetComponent<Enemy>().SetTarget(target);
        index++;
        
    }

    public List<GameObject> GetSpawnedEnemies()
    {
        return spawnedEnemies;
    }


}
