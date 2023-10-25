using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    [Tooltip("카메라가 바라보는 오브젝트")]
    public Transform target;
    [Tooltip("카메라와 목표사이의 거리")]
    public Vector3 offset = new Vector3(0, 3, -6);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 목표가 올바른 오브젝트인지 확인
        if (target != null)
        {
            // 목표와 일정한 거리를 두고 카메라위치 설정
            transform.position = target.position + offset;
            // 목표물을 바라보도록 회전 설정
            transform.LookAt(target);

        }
    }
}
