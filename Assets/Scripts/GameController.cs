using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Tooltip(" �����ϰ����ϴ� Ÿ�� ���� ")]
    public Transform tile;

    [Tooltip(" �����ϰ����ϴ� ��ֹ� ���� ")]
    public Transform obstacle;

    [Tooltip(" ù��° Ÿ���� �����Ǵ� �� ")]
    public Vector3 startPoint = new Vector3(0, 0, -5);
    [Tooltip(" ������ �����Ǵ� Ÿ���� �� ")]
    [Range(1, 15)]
    public int initSpawnNum = 10;
    [Tooltip(" ���� Ÿ���� ���� �Ǵ� ��ġ ")]
    private Vector3 nextTileLocation;
    [Tooltip(" ���� Ÿ���� ���� �Ǵ� ȸ���� ")]
    private Quaternion nextTileRotation;

    [Tooltip(" ��ֹ� ���� �����Ǵ� Ÿ���� �� �� ")]
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
    /// Ư����ġ�� Ÿ���� �����ϰ� ���� ��ġ�� �����Ѵ�.
    /// </summary>
    /// <param name="spawnObstacles">obstacle�� �����ؾ� �Ѵٸ� �Ķ���͸� ���</param>

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
        // ���� ��ֹ��� ������ �� �ִ� ������ ����Ҹ� Ȯ��.
        var obstacleSpawnPoints = new List<GameObject>();
        // Ÿ�Ͽ� �ִ� �� ���� ���� ��ü�� ���캸����.
        foreach (Transform child in newTile)
        {
            // ���� ������Ʈ�߿� ObstacleSpawn tag�� �ִ��� Ȯ���Ͽ�
            if (child.CompareTag("ObstacleSpawn"))
            {
                // �±װ��ִ� ��ġ�� Tile�� �߰��Ѵ�.
                obstacleSpawnPoints.Add(child.gameObject);
            }
        }
        // �ϳ� �̻� �ִ��� Ȯ���Ͽ�
        if (obstacleSpawnPoints.Count > 0)
        {
            // ������ ���� ������ obstacle ��ġ�� �����Ѵ�. 
            var spawnPoint = obstacleSpawnPoints[Random.Range(0, obstacleSpawnPoints.Count)];
            // ���� ������ ��ġ�� �����մϴ�. 
            var spawnPos = spawnPoint.transform.position;
            // obstacle ����
            var newObstacle = Instantiate(obstacle, spawnPos, Quaternion.identity);
            // ���� ������ Ÿ���� �θ�� ����
            newObstacle.SetParent(spawnPoint.transform);
        }
    }
}
