using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyProximityDetector : MonoBehaviour
{
    public float detectionRadius = 5f;         // ���� �ݰ�
    public string targetSceneName = "Test"; // ��ȯ�� �� �̸�
    public string enemyTag = "Enemy";          // �� �±�

    private bool triggered = false;

    void Update()
    {
        if (triggered) return;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance <= detectionRadius)
            {
                Debug.Log("�� ��ó ������. �� ��ȯ!");
                triggered = true;
                SceneManager.LoadScene(targetSceneName);
                break;
            }
        }
    }
}
