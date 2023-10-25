using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 
/// Player�� Tile End�� �������� �� ���ο� Ÿ�� ������ �ش�Ÿ���� �����Ѵ�.
/// </summary> 
public class TileEndBehaviour : MonoBehaviour
{
    [Tooltip(" EndTile�� ������ �� Ÿ���� �ı��ϱ� ������ ��ٷ��� �ϴ� �ð� ")]
    public float destroyTime = 1.5f;

    
    void OnTriggerEnter(Collider col)
    {
        // ���� �÷��̾�� �浹�ߴ��� üũ
        if (col.gameObject.GetComponent<PlayerBehaviour>())
        {
            // �浹������ ���ο�Ÿ���� �����Ѵ�.
            GameObject.FindObjectOfType<GameController>().SpawnNextTile();
            // �׸��� ����Ÿ���� 1.5���Ŀ� �����Ѵ�. 
            Destroy(transform.parent.gameObject, destroyTime);
        }
    }

}
