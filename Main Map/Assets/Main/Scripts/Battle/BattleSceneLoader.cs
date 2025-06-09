using UnityEngine;

public class BattleSceneLoader : MonoBehaviour
{
    void Start()
    {
        // 1. ��� ����
        GameObject background = Resources.Load<GameObject>("Backgrounds/Battle_Background");
        if (background != null)
            Instantiate(background, Vector3.zero, Quaternion.identity);

        // 2. �÷��̾� ����
        GameObject player = Resources.Load<GameObject>("Player/Player_Unit");
        if (player != null)
            Instantiate(player, new Vector3(-3, 0, 0), Quaternion.identity);

        // 3. �� ����
        string enemyName = BattleManager.selectedEnemyName;
        if (!string.IsNullOrEmpty(enemyName))
        {
            GameObject enemyPrefab = Resources.Load<GameObject>("Enemies/" + enemyName);
            if (enemyPrefab != null)
            {
                Instantiate(enemyPrefab, new Vector3(3, 0, 0), Quaternion.identity);
            }
            else
            {
                Debug.LogError("�������� ã�� �� �����ϴ�: " + enemyName);
            }
        }
    }
}
