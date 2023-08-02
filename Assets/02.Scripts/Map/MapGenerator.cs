using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("[Tile]")]
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Vector2 mapSize;

    [Header("[장애물 프리팹]")]
    public Transform obstaclePrefab;


    [Header("[Map Outline Percent]")]
    [Range(0,1)]
    [SerializeField] private float outlinePercent;

    [Header("[Seed]")]
    [SerializeField] private int seed = 10;

    private List<Coord> allTileCoords;
    private Queue<Coord> shuffledTileCoords;


    private void Start()
    {
        GenerateMap();
    }


    public void GenerateMap()
    {
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utiliy.ShuffleArray(allTileCoords.ToArray(), seed));

        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                // x좌표 0을 중심으로 맵의 가로 길이의 절반 만큼 왼쪽으로 이동한 점에서 부터 타일 생성(타일이 겹치지 않도록 0.5f + x를 더함)
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90), mapHolder) as Transform;

                newTile.localScale = Vector3.one * (1 - outlinePercent);
            }
        }

        // 생성할 장애물 수 특정
        int obstacleCount = 10;
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
            Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity, mapHolder) as Transform;
        }
    }

    private Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
    }

    /// <summary>
    /// 랜덤 좌표 반환
    /// </summary>
    public Coord GetRandomCoord()
    {
        // 셔플된 타일 큐의 첫 아이템을 가지도록 호출
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);

        return randomCoord;
    }

    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
    }
}
