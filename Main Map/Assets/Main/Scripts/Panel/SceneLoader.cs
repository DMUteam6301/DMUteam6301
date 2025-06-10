using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // ���� �޴� UI ��ü�� ���� (��: Canvas ��)
    public GameObject uiRoot;

    public void LoadWaitingRoom()
    {
        // 1. UI ��� ��Ȱ��ȭ
        if (uiRoot != null)
        {
            uiRoot.SetActive(false);
        }

        // 2. WaitingRoom ���� Additive �ε� (�񵿱�)
        SceneManager.LoadSceneAsync("WaitingRoom", LoadSceneMode.Additive).completed += (op) =>
        {
            // 3. WaitingRoom ���� Ȱ��ȭ ������ ����
            Scene loadedScene = SceneManager.GetSceneByName("WaitingRoom");
            SceneManager.SetActiveScene(loadedScene);

            // 4. MainMenu �� ��ε�
            SceneManager.UnloadSceneAsync("MainMenu");
        };
    }
}
