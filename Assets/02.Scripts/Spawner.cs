using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Wave[] waves;

    [SerializeField] private Enemy enemy;

    // �����ִ� ������ ��
    private int enemyiesRemainingToSpawn;

    // ��� �ִ� ���� ��
    private int enemiesRemaningAlive;

    // ������ ���� �ð�
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

    // ���� ���̺��� ������ ������ Ŭ����
    [Serializable]
    public class Wave
    {
        public int enemyCount;          // ���� ��
        public float timeBetweenSpawns; // ���� ����
    }


}
