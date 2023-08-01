using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Header("[Tile]")]
    [SerializeField] private Transform tilePrefab;
    [SerializeField] private Vector2 mapSize;

    [Header("[Map Outline Percent]")]
    [Range(0,1)]
    [SerializeField] private float outlinePercent;


    private void Start()
    {
        GenerateMap();
    }


    public void GenerateMap()
    {
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
                Vector3 tilePosition = new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y/2 + 0.5f + y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90), mapHolder) as Transform;

                newTile.localScale = Vector3.one * (1 - outlinePercent);
            }
        }
    }
}
