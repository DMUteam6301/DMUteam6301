/*using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public Transform target; // �÷��̾�
    public float sensor = 2f; // ���콺 ����
    public float maxCam = 90f; // ī�޶� ���� �ִ� ����

    public Vector3 offset = new Vector3(0, 1.6f, 0);

    float verticalRotation = 0f;
    float horizontalRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // ���콺 ����
    }

    void Update()
    {
        RotateCam();
    }

    void RotateCam()
    {
        // ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * sensor;
        float mouseY = Input.GetAxis("Mouse Y") * sensor;

        // �¿�
        horizontalRotation += mouseX;
        target.rotation = Quaternion.Euler(0, horizontalRotation, 0);

        // ī�޶�(���� ȸ��)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -maxCam, maxCam);
        transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        target.rotation = Quaternion.Euler(0, -horizontalRotation, 0);

        // ī�޶� ��ġ�� �÷��̾�� ����ȭ
        transform.position = target.position + target.TransformDirection(offset);
    }
}*/
/*using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerCam : MonoBehaviour
{
    public Transform target;  // �÷��̾�
    public Vector3 offset;    // ī�޶� ������

    public float mouseSensitivity = 2f; // ���콺 ����
    private float xRotation = 0f;       // ���� ȸ�� ��
    private float yRotation = 0f;       // �¿� ȸ�� ��

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // ���콺 Ŀ�� ����
    }

    void Update()
    {
        LookAround();
    }

    void LookAround()
    {
        // ���콺 �Է� �ޱ�
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // �¿�(Y��) ȸ��
        yRotation += mouseX;

        // ����(X��) ȸ�� (90�� ����)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // ī�޶� ��ġ�� �÷��̾� �������� ������Ʈ
        transform.position = target.position + offset;
        transform.rotation = target.rotation;
    }
}*/
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public Transform player;  // �÷��̾ ���� ���
    public Vector3 offset;
    public float mouseSensor = 2f;

    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensor;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensor;

        // ���� ȸ�� (ī�޶� ȸ��)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // �¿� ȸ�� (ī�޶� + �÷��̾� �Բ� ȸ��)
        yRotation += mouseX;

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.position = player.position + offset;

        // �÷��̾ ī�޶��� Y�� ȸ���� ���󰡵��� ����
        player.rotation = Quaternion.Euler(0, yRotation, 0);
    }
}
