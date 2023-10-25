using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleBehaviour : MonoBehaviour
{
    [Tooltip("게임을 다시 시작하기까지 기다리는 시간")]
    public float waitTime = 2.0f;
    public GameObject explosion;

    void OnCollisionEnter(Collision collision)
    {
        // 먼저 플레이어와 충돌했는지 확인하세요.
        if (collision.gameObject.GetComponent<PlayerBehaviour>())
        {
            // player 제거
            Destroy(collision.gameObject);

            // waitTime이 경과한 후 ResetGame 함수를 호출하세요.
            Invoke("ResetGame", waitTime);
        }
    }
    /// <summary> 
    /// 현재 로드된 레벨을 다시 시작합니다.
    /// </summary> 
    void ResetGame()
    {
        // 현재 레벨을 다시 시작합니다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void PlayerTouch()
    {
        if (explosion != null)
        {
            var particles = Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(particles, 0.1f);
        }
        Destroy(this.gameObject);
    }

}
