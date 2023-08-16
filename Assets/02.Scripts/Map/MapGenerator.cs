using System;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("[Maps]")]
    [SerializeField] private Map[] maps;
    [SerializeField] private int mapIndex;

    [Header("[Tile]")]
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Vector2 maxMapSize;

    [Header("[NavMashFloor]")]
    [SerializeField] private Transform navmashFloor;
    [SerializeField] private Transform navmeshMaskPrefab;

    [Header("[Obstracle(장애물)]")]
    public Transform obstaclePrefab;
    
    [Header("[Map Outline Percent]")]
    [Range(0,1)]
    [SerializeField] private float outlinePercent;

    [Header("[Tile Size]")]
    [SerializeField] private float tileSize;

    private List<Coord> allTileCoords;
    private Queue<Coord> shuffledTileCoords;
    private Map currentMap;


    private void Start()
    {
        GenerateMap();
    }


    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        System.Random prng = new System.Random(currentMap.seed);
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, 0.05f, currentMap.mapSize.y * tileSize);

        // 좌표(Coord) 생성
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utiliy.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        // 맵 홀더 오브젝트 생성
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // 타일 생성
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                // x좌표 0을 중심으로 맵의 가로 길이의 절반 만큼 왼쪽으로 이동한 점에서 부터 타일 생성(타일이 겹치지 않도록 0.5f + x를 더함)
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90), mapHolder) as Transform;

                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
            }
        }

        // 장애물을 생성하기 전, 맵사이즈 크기인 2차원 배열 bool을 선언
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        // 생성할 장애물 수 특정
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);

        // 현재 장애물 수
        int currentObstacleCont = 0;

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();

            // 장애물 위치 저장
            obstacleMap[randomCoord.x, randomCoord.y] = true;

            currentObstacleCont++;

            // 맵 중앙에 생성 x
            if (randomCoord != currentMap.mapCenter && MaplsFullyAccessible(obstacleMap, currentObstacleCont))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());  

                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight/2, Quaternion.identity, mapHolder) as Transform;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);

                Renderer obstracleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstracleMaterial = new Material(obstracleRenderer.sharedMaterial);
                float colourPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstracleMaterial.color = Color.Lerp(currentMap.foregroundColour, currentMap.backgroundColour, colourPercent);
                obstracleRenderer.sharedMaterial = obstracleMaterial;
            }
            else
            {
                // 조건이 맞지 않아 장애물을 생성하지 못했을 시
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCont--;
            }
        }

        // 네브메쉬 마스크 동적으로 설정
        Transform maskLeft = Instantiate(navmeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity, mapHolder) as Transform;
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.y) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navmeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity, mapHolder) as Transform;
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.y) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navmeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity, mapHolder) as Transform;
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBottom = Instantiate(navmeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity, mapHolder) as Transform;
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;


        navmashFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }

    /// <summary>
    /// 맵 전체가 접근 가능한지 확인 (Flood Fill 알고리즘) 
    /// </summary>
    /// <param name="obstacleMap">장애물 맵 </param>
    /// <param name="currentObstacleCuont">지금까지 장애물이 얼마나 생성됬는지</param>
    /// <returns></returns>
    private bool MaplsFullyAccessible(bool[,] obstacleMap, int currentObstacleCuont)
    {
        // 순회 시작준비 (맵 중앙부터 시작[ampCenter])
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCenter);
        mapFlags[currentMap.mapCenter.x, currentMap.mapCenter.y] = true;

        // 접근 가능한 타일의 수
        int accessibleTileCount = 1;

        while(queue.Count > 0)
        {
            // 이 좌표에 근접한 네개의 이웃 타일들을 루프
            Coord tile = queue.Dequeue();

            // 이웃 타일의 상하좌우를 순환 (-1부터 1까지 돌기 때문)
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;

                    // 대각선을 체크하지 않음
                    if (x == 0 || y == 0)
                    {
                        // 맵 밖으로 나가지 않도록 (좌표가 맵 내부에 있는지 확인)
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            // 만약 이 타일을 이전에 체크 하지 않았다면 && 장애물이 없다면
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                // 타일을 체크하고 Queue에 담아 다시 루프
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        // 현재 타일수 - 장애물 수
        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCuont);

        // 장애물을 뺀 타일의 값과 접근 가능한 타일의 수가 같을 경우 true
        return targetAccessibleTileCount == accessibleTileCount;
    }

    private Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
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

    [Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }


    [Serializable]
    public class Map
    {
        [Header("[Map의 2차원 크기]")]
        public Coord mapSize;

        [Header("[Obstracle(장애물) 수]")]
        [Range(0,1)]
        public float obstaclePercent;

        [Header("[Seed]")]
        public int seed;

        [Header("[장애물의 높이 제한]")]
        public float minObstacleHeight;
        public float maxObstacleHeight;

        [Header("[장애물 전면부 후면부 Color]")]
        public Color foregroundColour;
        public Color backgroundColour;

        // 플레이어 생성을 위한 맵 중앙 변수
        public Coord mapCenter { get { return new Coord(mapSize.x / 2, mapSize.y / 2); } }
    }
}
