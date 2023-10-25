using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// Player가 Tile End에 도착했을 때 새로운 타일 생성과 해당타일을 제거한다.
/// </summary> 
public class TileEndBehaviour : MonoBehaviour
{
    [Tooltip(" EndTile에 도달한 후 타일을 파괴하기 전까지 기다려야 하는 시간 ")]
    public float destroyTime = 1.5f;

    
    void OnTriggerEnter(Collider col)
    {
        // 먼저 플레이어와 충돌했는지 체크
        if (col.gameObject.GetComponent<PlayerBehaviour>())
        {
            // 충돌했으면 새로운타일을 생성한다.
            GameObject.FindObjectOfType<GameController>().SpawnNextTile();
            // 그리고 현재타일을 1.5초후에 제거한다. 
            Destroy(transform.parent.gameObject, destroyTime);
        }
    }

}
