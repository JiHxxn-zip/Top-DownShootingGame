using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private Wave[] waves;

    [SerializeField] private Enemy enemy;

    // 플레이어 정보
    private LivingEntity playerEntity;
    // 플레이어 위치를 계속 추적할
    private Transform playerT;

    // 남아있는 스폰할 적
    private int enemyiesRemainingToSpawn;

    // 살아 있는 적의 수
    private int enemiesRemaningAlive;

    // 다음번 스폰 시간
    private float nextSpawnTime;

    private Wave currentWaves;
    private int currentWaveNumver;

    private MapGenerator map;

    // 플레이어가 같은 자리에 오래있을 경우를 방지하여 검사할 간격 수
    private float timeBetweenCampingChecks = 2;
    // 검사시 움직여야할 최소 한계거리
    private float campThresholdDistance = 1.5f;
    // 다음 검사 예정시간
    private float nextCampCheckTime;
    // 최근 검사시 플레이어가 있던 장소
    private Vector3 campPositionOld;
    // 검사여부
    private bool isCamping;

    // 플레이어가 죽었을 경우 비활성화
    private bool isDisabled;

    // 새 웨이브가 시작될 때 알려줄 이벤트
    public event Action<int> OnNewWave;


    private void Start()
    {
        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    private void Update()
    {
        if (isDisabled == false)
        {
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = Time.time + timeBetweenCampingChecks;

                // 플레이어의 과거 위치와 현재위치 검사
                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }

            if (enemyiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemyiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWaves.timeBetweenSpawns;

                StartCoroutine(SpawnEnemy());
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;

        Transform randomtile = map.GetRandomOpenTile();
        // 플레이어가 한자리에 오래있을 경우 랜덤위치가 아니라 플레이어 근처 타일로 이동
        if(isCamping)
        {
            randomtile = map.GetTileFromPosition(playerT.position);
        }

        Material tileMat = randomtile.GetComponent<Renderer>().material;
        Color initialColur = tileMat.color;
        Color fleshColour = Color.red;
        float spawnTimer = 0;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColur, fleshColour, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnedEnemy = Instantiate(enemy, randomtile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
    }

    private void OnPlayerDeath()
    {
        isDisabled = true;
    }

    private void OnEnemyDeath()
    {
        enemiesRemaningAlive--;

        if(enemiesRemaningAlive == 0)
        {
            NextWave();
        }
    }

    private void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    private void NextWave()
    {
        currentWaveNumver++;

        if(currentWaveNumver -1 < waves.Length)
        {
            currentWaves = waves[currentWaveNumver - 1];

            enemyiesRemainingToSpawn = currentWaves.enemyCount;
            enemiesRemaningAlive = enemyiesRemainingToSpawn;

            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumver);
            }
            ResetPlayerPosition();
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
