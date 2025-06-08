using Unity.Netcode;
using UnityEngine;

public class NetworkedPlayerController : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float mouseSensor = 2f;
    [SerializeField] private float interactDistance = 1f;
    [SerializeField] private float jumpForce = 7.5f;

    [Header("Object Grabbing")]
    [SerializeField] private float grabRange = 3f;
    [SerializeField] private float grabRadius = 1.5f;
    [SerializeField] private float moveSpeed = 10f;

    // �÷��̾� ������Ʈ��
    private CharacterController characterController;
    private Camera playerCamera;
    private AudioListener audioListener;
    private Transform holdPosition;

    // �Է� ������
    private float hAxis;
    private float vAxis;
    private float mouseX;
    private bool wDown;
    private bool jDown;
    private bool isJump;
    private Vector3 moveVec;
    private Vector3 velocity;

    // ������Ʈ ���
    private GameObject grabbedObject;
    private Rigidbody grabbedRb;
    private bool isBlocked = false;
    private string originalTag;

    // �� �÷��̾ � Ŭ���̾�Ʈ������ �ĺ�
    [Header("Player ID")]
    [SerializeField] private bool isPlayer1 = true; // Player (1)�̸� true, Player (2)�� false

    private void Awake()
    {
        // ������Ʈ ��������
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        audioListener = GetComponentInChildren<AudioListener>();

        // holdPosition ����
        if (playerCamera != null)
        {
            GameObject holdPosObject = new GameObject("HoldPosition");
            holdPosition = holdPosObject.transform;
            holdPosition.SetParent(playerCamera.transform);
            holdPosition.localPosition = new Vector3(0, 0, 2f);
        }

        Debug.Log($"Player Controller Awake: {gameObject.name}, Camera found: {playerCamera != null}");
    }

    public override void OnNetworkSpawn()
    {
        // �� �÷��̾ ������ �� �ִ� Ŭ���̾�Ʈ ����
        bool shouldControlThisPlayer = false;

        if (isPlayer1 && IsHost) // Player (1)�� ȣ��Ʈ�� ����
        {
            shouldControlThisPlayer = true;
        }
        else if (!isPlayer1 && IsClient && !IsHost) // Player (2)�� Ŭ���̾�Ʈ�� ����
        {
            shouldControlThisPlayer = true;
        }

        if (shouldControlThisPlayer)
        {
            SetupAsControlledPlayer();
        }
        else
        {
            SetupAsRemotePlayer();
        }

        Debug.Log($"NetworkSpawn: {gameObject.name}, IsOwner: {IsOwner}, ShouldControl: {shouldControlThisPlayer}, IsHost: {IsHost}");
    }

    private void SetupAsControlledPlayer()
    {
        // ��� ī�޶�� ����� ������ ��Ȱ��ȭ
        DisableAllCamerasAndAudio();

        // �� �÷��̾��� ī�޶�� ������� Ȱ��ȭ
        if (playerCamera != null)
        {
            playerCamera.enabled = true;
            playerCamera.tag = "MainCamera";
        }

        if (audioListener != null)
        {
            audioListener.enabled = true;
        }

        // Ŀ�� ���
        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log($"Setup as controlled player: {gameObject.name}");
    }

    private void SetupAsRemotePlayer()
    {
        // ���� �÷��̾�� �Է� ��Ȱ��ȭ
        enabled = false;

        // ī�޶�� ����� ��Ȱ��ȭ
        if (playerCamera != null)
        {
            playerCamera.enabled = false;
        }

        if (audioListener != null)
        {
            audioListener.enabled = false;
        }

        Debug.Log($"Setup as remote player: {gameObject.name}");
    }

    private void DisableAllCamerasAndAudio()
    {
        // ��� ī�޶� ��Ȱ��ȭ
        Camera[] allCameras = FindObjectsOfType<Camera>(true);
        foreach (Camera cam in allCameras)
        {
            cam.enabled = false;
        }

        // ��� ����� ������ ��Ȱ��ȭ
        AudioListener[] allListeners = FindObjectsOfType<AudioListener>(true);
        foreach (AudioListener listener in allListeners)
        {
            listener.enabled = false;
        }
    }

    private void Update()
    {
        if (!enabled) return;

        GetInput();
        Move();
        LookAround();
        Jump();
        HandleObjectGrabbing();

    }

    private void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        jDown = Input.GetButtonDown("Jump");
        wDown = Input.GetKey(KeyCode.LeftShift);
        mouseX = Input.GetAxis("Mouse X") * mouseSensor;

        // ESCŰ�� Ŀ�� ����/���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Move()
    {
        if (characterController == null) return;

        // ���� Ȯ��
        bool isGrounded = characterController.isGrounded;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        forward.y = 0;
        right.y = 0;

        moveVec = (forward * vAxis + right * hAxis).normalized;
        float currentMoveSpeed = speed * (wDown && !isJump ? 1.5f : 1f);

        // ���� �̵�
        Vector3 horizontalMove = moveVec * currentMoveSpeed * Time.deltaTime;

        // �߷� ó��
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y += -9.81f * Time.deltaTime;
        }

        // ���� �̵�
        Vector3 finalMove = horizontalMove + new Vector3(0, velocity.y * Time.deltaTime, 0);
        characterController.Move(finalMove);
    }

    private void LookAround()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // �¿� ȸ��
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Jump()
    {
        if (jDown && characterController != null && characterController.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * -9.81f);
            isJump = true;
        }

        if (characterController != null && characterController.isGrounded)
        {
            isJump = false;
        }
    }

    private void HandleObjectGrabbing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (grabbedObject == null)
            {
                TryGrabObject();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (grabbedObject != null)
            {
                ReleaseObject();
            }
        }

        if (grabbedObject && !isBlocked)
        {
            MoveObjectWithPlayer();
        }
    }

    private void TryGrabObject()
    {
        if (playerCamera == null) return;

        Vector3 sphereCenter = playerCamera.transform.position + playerCamera.transform.forward * grabRange / 2;
        Collider[] colliders = Physics.OverlapSphere(sphereCenter, grabRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Grabbable"))
            {
                grabbedObject = collider.gameObject;
                grabbedRb = grabbedObject.GetComponent<Rigidbody>();

                if (grabbedRb)
                {
                    grabbedRb.useGravity = false;
                    grabbedRb.freezeRotation = true;
                    grabbedRb.isKinematic = true;
                }

                originalTag = grabbedObject.tag;
                grabbedObject.tag = "Untagged";

                return;
            }
        }
    }

    private void MoveObjectWithPlayer()
    {
        if (grabbedObject && holdPosition)
        {
            grabbedObject.transform.position = Vector3.Lerp(
                grabbedObject.transform.position,
                holdPosition.position,
                Time.deltaTime * moveSpeed
            );
        }
    }

    private void ReleaseObject()
    {
        if (grabbedRb)
        {
            grabbedRb.useGravity = true;
            grabbedRb.freezeRotation = false;
            grabbedRb.isKinematic = false;
        }

        if (grabbedObject)
        {
            grabbedObject.tag = originalTag;
        }

        grabbedObject = null;
        grabbedRb = null;
        isBlocked = false;
    }

}