using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Wave[] waves;

    [SerializeField] private Enemy enemy;

    // 남아있는 스폰할 적
    private int enemyiesRemainingToSpawn;

    // 살아 있는 적의 수
    private int enemiesRemaningAlive;

    // 다음번 스폰 시간
    private float nextSpawnTime;

    private Wave currentWaves;
    private int currentWaveNumver;


    private void Start()
    {
        NextWave();
    }

    private void Update()
    {
        if(enemyiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemyiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWaves.timeBetweenSpawns;

            Enemy spawnedEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }
    }

    private void OnEnemyDeath()
    {
        enemiesRemaningAlive--;

        if(enemiesRemaningAlive == 0)
        {
            NextWave();
        }
    }

    private void NextWave()
    {
        currentWaveNumver++;

        Debug.Log("Wave : " + currentWaveNumver);

        if(currentWaveNumver -1 < waves.Length)
        {
            currentWaves = waves[currentWaveNumver - 1];

            enemyiesRemainingToSpawn = currentWaves.enemyCount;
            enemiesRemaningAlive = enemyiesRemainingToSpawn;
        }

    }

    // 내부 웨이브의 정보를 저장할 클래스
    [Serializable]
    public class Wave
    {
        public int enemyCount;          // 적의 수
        public float timeBetweenSpawns; // 스폰 간격
    }


}
