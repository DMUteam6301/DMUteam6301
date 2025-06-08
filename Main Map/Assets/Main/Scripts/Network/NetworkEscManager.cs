using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class NetworkEscManager : NetworkBehaviour
{
    public GameObject CancelPanel;
    private bool activeCancel = false;

    // �� ESC Manager�� � �÷��̾������ �ĺ�
    [Header("Player Assignment")]
    public bool isForPlayer1 = true; // Player (1)���̸� true, Player (2)���̸� false

    private void Start()
    {
        // �ʱ� ����: Canvas ��Ȱ��ȭ
        if (CancelPanel != null)
        {
            CancelPanel.SetActive(false);
            activeCancel = false;
        }

        Debug.Log($"NetworkEscManager Start - {gameObject.name}, IsForPlayer1: {isForPlayer1}, Panel: {CancelPanel?.name}");
    }

    public override void OnNetworkSpawn()
    {
        // �� �÷��̾ �ڽ��� ESC Manager�� ������ �� �ֵ��� ����
        bool shouldControlThisManager = false;

        if (isForPlayer1 && IsHost) // Player (1)�� Manager�� ȣ��Ʈ�� ����
        {
            shouldControlThisManager = true;
        }
        else if (!isForPlayer1 && IsClient && !IsHost) // Player (2)�� Manager�� Ŭ���̾�Ʈ�� ����
        {
            shouldControlThisManager = true;
        }

        if (!shouldControlThisManager)
        {
            // �ٸ� �÷��̾��� ESC Manager�� ��Ȱ��ȭ
            enabled = false;
        }

        Debug.Log($"NetworkEscManager OnNetworkSpawn - {gameObject.name}, ShouldControl: {shouldControlThisManager}, IsHost: {IsHost}");
    }

    private void Update()
    {
        // enabled�� false�� ������� ���� (�ٸ� �÷��̾��� Manager)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleEscMenu();
        }
    }

    private void ToggleEscMenu()
    {
        activeCancel = !activeCancel;

        if (CancelPanel != null)
        {
            CancelPanel.SetActive(activeCancel);
        }

        // Ŀ�� ���� ����
        Cursor.lockState = activeCancel ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = activeCancel;

        Debug.Log($"ESC Menu toggled - {gameObject.name}, Active: {activeCancel}");
    }

    public void GameSave()
    {
        // ��Ƽ�÷��̾���� ���� ��� ��Ȱ��ȭ
        Debug.Log("Save functionality disabled in multiplayer");
        CloseMenu();
    }

    public void GameLoad()
    {
        // ��Ƽ�÷��̾���� �ε� ��� ��Ȱ��ȭ
        Debug.Log("Load functionality disabled in multiplayer");
        CloseMenu();
    }

    public void GameExit()
    {
        // ��Ʈ��ũ ���� ����
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // �κ�� ���ư���
        SceneManager.LoadScene("LobbyScene");
    }

    public void CloseMenu()
    {
        activeCancel = false;
        if (CancelPanel != null)
            CancelPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log($"ESC Menu closed - {gameObject.name}");
    }
}