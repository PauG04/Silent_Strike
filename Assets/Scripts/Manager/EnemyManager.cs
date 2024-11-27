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
    [SerializeField] private EnemyWave bossWave;
    [SerializeField] private List<Transform> spawnPosition;
    private int index;
    private bool bossActive;

    [Header("Enemies")]
    private List<GameObject> spawnedEnemies;

    [Header("Player")]
    [SerializeField] private GameObject target;

    [Header("Time")]
    private float currentTime;

    [Header("Sounds")]
    [SerializeField] private AudioClip smokeBombSound;

    private void Start()
    {
        index = 0;
        spawnedEnemies = new List<GameObject>();
        bossActive = false;
    }
    private void Update()
    {
        currentTime += Time.deltaTime;
        if (index < enemyWave.enemies.Count && !bossActive)
        {
            if (enemyWave.enemies[index].spawnTime < currentTime || (spawnedEnemies.Count == 0 && index != 0))
            {
                SpawnEnemy(enemyWave.enemies[index].enemyPrefab);
                currentTime = 0f;
            }
        }
        else
        {
            LevelManager.instance.SetCanCangeLevel(true);
        }

        if (Input.GetKeyDown(KeyCode.B) && !bossActive)
        {
            for(int i = 0; i<spawnedEnemies.Count; i++)
            {
                spawnedEnemies[i].GetComponent<Enemy>().ReceiveDamageEnemy(100000, true);
            }

            index = 0;
            SpawnEnemy(bossWave.enemies[index].enemyPrefab);
            bossActive = true;
        }
    }

    public void SpawnEnemy(GameObject _enemy)
    {
        GameObject enemy = Instantiate(_enemy);

        int randomSpawnPoint = Random.Range(0, spawnPosition.Count);

        enemy.transform.position = new Vector3(spawnPosition[randomSpawnPoint].position.x,
            spawnPosition[randomSpawnPoint].position.y + enemy.GetComponent<SpriteRenderer>().bounds.size.y / 2,
            spawnPosition[randomSpawnPoint].position.z);

        AudioManager.instance.Play2dOneShotSound(smokeBombSound, "Sfx");
        spawnPosition[randomSpawnPoint].GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();
        spawnPosition[randomSpawnPoint].GetChild(0).GetChild(0).GetChild(0).GetComponent<ParticleSystem>().Play();

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

    public GameObject GetLastEnemy()
    {
        return spawnedEnemies[spawnedEnemies.Count - 1];
    }

    public void DeleteEnemy(GameObject enemy)
    {
        spawnedEnemies.Remove(enemy);
    }


}
