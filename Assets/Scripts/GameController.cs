using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Tooltip(" 생성하고자하는 타일 참조 ")]
    public Transform tile;

    [Tooltip(" 생성하고자하는 장애물 참조 ")]
    public Transform obstacle;

    [Tooltip(" 첫번째 타일이 생성되는 곳 ")]
    public Vector3 startPoint = new Vector3(0, 0, -5);
    [Tooltip(" 사전에 생성되는 타일의 수 ")]
    [Range(1, 15)]
    public int initSpawnNum = 10;
    [Tooltip(" 다음 타일이 생성 되는 위치 ")]
    private Vector3 nextTileLocation;
    [Tooltip(" 다음 타일이 생성 되는 회전값 ")]
    private Quaternion nextTileRotation;

    [Tooltip(" 장애물 없이 생성되는 타일의 갯 수 ")]
    public int initNoObstacle = 4;



    // Start is called before the first frame update
    void Start()
    {
        nextTileLocation = startPoint;
        nextTileRotation = Quaternion.identity;

        for (int i = 0; i < initSpawnNum; ++i)
        {
            SpawnNextTile( i >= initNoObstacle);
        }
    }

     /// <summary>
    /// 특정위치에 타일을 생성하고 다음 위치를 생성한다.
    /// </summary>
    /// <param name="spawnObstacles">obstacle을 생성해야 한다면 파라미터를 사용</param>

    public void SpawnNextTile(bool spawnObstacles = true)
    {
        var newTile = Instantiate(tile, nextTileLocation, nextTileRotation);
        var nextTile = newTile.Find("Next Spawn Point");
        nextTileLocation = nextTile.position;
        nextTileRotation = nextTile.rotation;

        if (spawnObstacles)
        {
            SpawnObstacle(newTile);
        }
    }
    void SpawnObstacle( Transform newTile)
    {
        // 이제 장애물을 생성할 수 있는 가능한 저장소를 확보.
        var obstacleSpawnPoints = new List<GameObject>();
        // 타일에 있는 각 하위 게임 개체를 살펴보세요.
        foreach (Transform child in newTile)
        {
            // 하위 오브젝트중에 ObstacleSpawn tag가 있는지 확인하여
            if (child.CompareTag("ObstacleSpawn"))
            {
                // 태그가있는 위치에 Tile을 추가한다.
                obstacleSpawnPoints.Add(child.gameObject);
            }
        }
        // 하나 이상 있는지 확인하여
        if (obstacleSpawnPoints.Count > 0)
        {
            // 임으로 새로 생성할 obstacle 위치를 설정한다. 
            var spawnPoint = obstacleSpawnPoints[Random.Range(0, obstacleSpawnPoints.Count)];
            // 새로 생성할 위치를 저장합니다. 
            var spawnPos = spawnPoint.transform.position;
            // obstacle 생성
            var newObstacle = Instantiate(obstacle, spawnPos, Quaternion.identity);
            // 새로 생성된 타일을 부모로 설정
            newObstacle.SetParent(spawnPoint.transform);
        }
    }
}
