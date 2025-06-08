using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;

public class NetworkPlayer : NetworkBehaviour
{
    [Header("Player Components")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private GameObject playerModel;
    [SerializeField] private Material[] playerMaterials;
    [SerializeField] private Camera playerCamera; // �����鿡 ���Ե� ī�޶�
    [SerializeField] private AudioListener audioListener; // �����鿡 ���Ե� ����� ������

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();
    private NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>();

    private Vector2 moveInput;
    private Vector3 velocity;
    private bool isGrounded;

    private CharacterController characterController;
    private Renderer playerRenderer;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (playerModel != null)
        {
            playerRenderer = playerModel.GetComponent<Renderer>();
        }

        // CharacterController ���� ����ȭ
        if (characterController != null)
        {
            characterController.stepOffset = 0.3f;
            characterController.slopeLimit = 45f;
            characterController.skinWidth = 0.08f;
        }

        // �����鿡�� ī�޶�� ����� ������ �ڵ� ã�� (�Ҵ���� ���� ���)
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();
        if (audioListener == null)
            audioListener = GetComponentInChildren<AudioListener>();
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"=== OnNetworkSpawn called for ClientID: {OwnerClientId}, IsOwner: {IsOwner} ===");

        networkPosition.OnValueChanged += OnPositionChanged;
        networkRotation.OnValueChanged += OnRotationChanged;
        playerName.OnValueChanged += OnPlayerNameChanged;

        if (IsOwner)
        {
            Debug.Log($"Setting up LOCAL player for ClientID: {OwnerClientId}");
            SetupLocalPlayer();

            string currentPlayerName = AuthenticationManager.Instance?.GetCurrentUserName() ?? $"Player{OwnerClientId}";
            SetPlayerNameServerRpc(currentPlayerName);

            // ���� ��ġ ����
            StartCoroutine(SetSpawnPositionCoroutine());
        }
        else
        {
            Debug.Log($"Setting up REMOTE player for ClientID: {OwnerClientId}");
            SetupRemotePlayer();
        }

        SetPlayerColor();

        Debug.Log($"NetworkPlayer spawned - ClientID: {OwnerClientId}, IsOwner: {IsOwner}, Position: {transform.position}");
    }

    public override void OnNetworkDespawn()
    {
        networkPosition.OnValueChanged -= OnPositionChanged;
        networkRotation.OnValueChanged -= OnRotationChanged;
        playerName.OnValueChanged -= OnPlayerNameChanged;
    }

    private void SetupLocalPlayer()
    {
        Debug.Log($"SetupLocalPlayer called for ClientID: {OwnerClientId}");

        // ��� �ٸ� ī�޶���� ��Ȱ��ȭ
        DisableAllOtherCameras();

        // �ڽ��� ī�޶�� ����� ������ Ȱ��ȭ
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
            playerCamera.tag = "MainCamera";
            Debug.Log($"Local player camera activated for ClientID: {OwnerClientId}");
        }
        else
        {
            Debug.LogError($"Player camera not found for ClientID: {OwnerClientId}");
        }

        if (audioListener != null)
        {
            // �ٸ� ��� AudioListener ��Ȱ��ȭ
            DisableAllOtherAudioListeners();
            audioListener.enabled = true;
            Debug.Log($"Local player audio listener activated for ClientID: {OwnerClientId}");
        }
        else
        {
            Debug.LogError($"Audio listener not found for ClientID: {OwnerClientId}");
        }
    }

    private void SetupRemotePlayer()
    {
        Debug.Log($"SetupRemotePlayer called for ClientID: {OwnerClientId}");

        // ���� �÷��̾��� ī�޶�� ����� ������ ��Ȱ��ȭ
        if (playerCamera != null)
        {
            playerCamera.enabled = false;
            Debug.Log($"Remote player camera disabled for ClientID: {OwnerClientId}");
        }

        if (audioListener != null)
        {
            audioListener.enabled = false;
            Debug.Log($"Remote player audio listener disabled for ClientID: {OwnerClientId}");
        }

        // �÷��̾� ���� ���̵��� ����
        if (playerModel != null)
        {
            playerModel.SetActive(true);
        }
    }

    private void DisableAllOtherCameras()
    {
        // ���� ��� ī�޶� ã�Ƽ� �ڽ��� ī�޶� �ƴ� ��� ��Ȱ��ȭ
        Camera[] allCameras = FindObjectsOfType<Camera>(true);
        foreach (Camera cam in allCameras)
        {
            if (cam != playerCamera && cam.gameObject.scene.isLoaded)
            {
                cam.enabled = false;
                Debug.Log($"Disabled other camera: {cam.name}");
            }
        }
    }

    private void DisableAllOtherAudioListeners()
    {
        // ���� ��� AudioListener�� ã�Ƽ� �ڽ��� ���� �ƴ� ��� ��Ȱ��ȭ
        AudioListener[] allListeners = FindObjectsOfType<AudioListener>(true);
        foreach (AudioListener listener in allListeners)
        {
            if (listener != audioListener && listener.gameObject.scene.isLoaded)
            {
                listener.enabled = false;
                Debug.Log($"Disabled other audio listener: {listener.name}");
            }
        }
    }

    private System.Collections.IEnumerator SetSpawnPositionCoroutine()
    {
        // ��Ʈ��ũ ����ȭ�� ���� ��� ���
        yield return new WaitForSeconds(0.1f);

        // GameManager�� �̹� �ùٸ� ��ġ�� ���������Ƿ� ���� ��ġ �״�� ���
        Vector3 currentPosition = transform.position;

        // ���θ� ���ֺ����� ȸ�� ����
        Quaternion spawnRotation = GetFacingRotation();
        transform.rotation = spawnRotation;

        // �ӵ� ���� �ʱ�ȭ
        velocity = Vector3.zero;

        // CharacterController ����
        if (characterController != null)
        {
            characterController.enabled = false;
            yield return null; // �� ������ ���
            characterController.enabled = true;
        }

        // ������ ��ġ�� ȸ�� ������Ʈ (���� ��ġ ����)
        if (IsServer)
        {
            networkPosition.Value = currentPosition;
            networkRotation.Value = spawnRotation;
        }
        else
        {
            UpdatePositionServerRpc(currentPosition, spawnRotation);
        }

        Debug.Log($"Player {OwnerClientId} spawned at GameManager position: {currentPosition}, rotation: {spawnRotation.eulerAngles}");
    }

    private Quaternion GetFacingRotation()
    {
        // Z�� �������� ���θ� ���ֺ����� ȸ��
        if (OwnerClientId == 0)
        {
            // ȣ��Ʈ(z:-300): +Z ����(����)�� �ٶ� (Ŭ���̾�Ʈ ������)
            return Quaternion.LookRotation(Vector3.forward);
        }
        else
        {
            // Ŭ���̾�Ʈ(z:150): -Z ����(����)�� �ٶ� (ȣ��Ʈ ������)
            return Quaternion.LookRotation(Vector3.back);
        }
    }

    private void SetPlayerColor()
    {
        if (playerRenderer != null && playerMaterials.Length > 0)
        {
            int materialIndex = (int)(OwnerClientId % (ulong)playerMaterials.Length);
            playerRenderer.material = playerMaterials[materialIndex];
            Debug.Log($"Player {OwnerClientId} color set to material index: {materialIndex}");
        }
    }

    private void Update()
    {
        if (!IsOwner) return;

        CheckGrounded();
        HandleInput();
        HandleMovement();

        if (HasMoved())
        {
            UpdatePositionServerRpc(transform.position, transform.rotation);
        }
    }

    private void CheckGrounded()
    {
        // �ܼ�ȭ: CharacterController�� isGrounded�� ���
        isGrounded = characterController != null && characterController.isGrounded;
    }

    private void HandleInput()
    {
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.y = Input.GetAxis("Vertical");
    }

    private void HandleMovement()
    {
        if (characterController == null || !characterController.enabled) return;

        // ���� �̵��� ó�� (�ܼ�ȭ)
        Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        // �̵� ���� ���
        Vector3 moveVector = moveDirection * moveSpeed * Time.deltaTime;

        // ȸ�� ó��
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // �ܼ��� �߷� ó��
        if (characterController.isGrounded)
        {
            // �ٴڿ� ���� ���� �߷��� ���� 0����
            velocity.y = -0.5f;
        }
        else
        {
            // ���߿� ���� ���� �߷� ����
            velocity.y += -9.81f * Time.deltaTime;
        }

        // ���� �̵�: ���� �̵� + ���� �̵�(�߷�)
        Vector3 finalMove = moveVector + new Vector3(0, velocity.y * Time.deltaTime, 0);

        // �̵� ����
        characterController.Move(finalMove);

        // ���������� ��Ȳ üũ �� ����
        if (transform.position.y < -10f) // �ʹ� �Ʒ��� ������ ���
        {
            Debug.LogWarning($"Player {OwnerClientId} fell too low, resetting position");
            Vector3 resetPos = new Vector3(transform.position.x, 110f, transform.position.z); // Y=110���� ���� (�ٴ� ����)
            transform.position = resetPos;
            velocity = Vector3.zero;
            UpdatePositionServerRpc(resetPos, transform.rotation);
        }
    }

    private bool HasMoved()
    {
        return Vector3.Distance(transform.position, networkPosition.Value) > 0.01f ||
               Quaternion.Angle(transform.rotation, networkRotation.Value) > 1f;
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector3 position, Quaternion rotation)
    {
        networkPosition.Value = position;
        networkRotation.Value = rotation;
    }

    [ServerRpc]
    private void SetPlayerNameServerRpc(string name)
    {
        playerName.Value = name;
    }

    private void OnPositionChanged(Vector3 oldValue, Vector3 newValue)
    {
        if (!IsOwner)
        {
            StartCoroutine(InterpolatePosition(newValue));
        }
    }

    private void OnRotationChanged(Quaternion oldValue, Quaternion newValue)
    {
        if (!IsOwner)
        {
            StartCoroutine(InterpolateRotation(newValue));
        }
    }

    private void OnPlayerNameChanged(FixedString64Bytes oldValue, FixedString64Bytes newValue)
    {
        if (playerNameText != null)
        {
            playerNameText.text = newValue.ToString();
        }
        Debug.Log($"Player name changed: {newValue} (ClientID: {OwnerClientId})");
    }

    private System.Collections.IEnumerator InterpolatePosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        float interpolationTime = 0.1f;

        while (elapsedTime < interpolationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / interpolationTime;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        transform.position = targetPosition;
    }

    private System.Collections.IEnumerator InterpolateRotation(Quaternion targetRotation)
    {
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;
        float interpolationTime = 0.1f;

        while (elapsedTime < interpolationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / interpolationTime;

            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return null;
        }

        transform.rotation = targetRotation;
    }

    public string GetPlayerName()
    {
        return playerName.Value.ToString();
    }

    public new bool IsLocalPlayer()
    {
        return IsOwner;
    }

    // ����׿� �޼���
    [ContextMenu("Teleport to Safe Position")]
    private void TeleportToSafePosition()
    {
        if (IsOwner)
        {
            // ���� ��ġ���� Y�� �ٴ� ���̷� ����
            Vector3 safePosition = new Vector3(transform.position.x, 110f, transform.position.z);
            transform.position = safePosition;
            velocity = Vector3.zero;
            UpdatePositionServerRpc(safePosition, transform.rotation);
            Debug.Log($"Player {OwnerClientId} teleported to safe position: {safePosition}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (characterController != null)
        {
            // Ground check �ð�ȭ (�ܼ�ȭ)
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector3 spherePosition = transform.position - Vector3.up * (characterController.height * 0.5f);
            Gizmos.DrawWireSphere(spherePosition, characterController.radius);
        }
    }
}