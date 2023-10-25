using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleBehaviour : MonoBehaviour
{
    [Tooltip("������ �ٽ� �����ϱ���� ��ٸ��� �ð�")]
    public float waitTime = 2.0f;
    public GameObject explosion;

    void OnCollisionEnter(Collision collision)
    {
        // ���� �÷��̾�� �浹�ߴ��� Ȯ���ϼ���.
        if (collision.gameObject.GetComponent<PlayerBehaviour>())
        {
            // player ����
            Destroy(collision.gameObject);

            // waitTime�� ����� �� ResetGame �Լ��� ȣ���ϼ���.
            Invoke("ResetGame", waitTime);
        }
    }
    /// <summary> 
    /// ���� �ε�� ������ �ٽ� �����մϴ�.
    /// </summary> 
    void ResetGame()
    {
        // ���� ������ �ٽ� �����մϴ�.
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
