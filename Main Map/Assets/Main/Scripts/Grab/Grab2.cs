using UnityEngine;

public class PlayerObjectGrabber : MonoBehaviour
{
    public Camera playerCamera;
    public Transform holdPosition;
    public float moveSpeed = 10f;
    public float grabRange = 3f;
    public float grabRadius = 1.5f;

    private GameObject grabbedObject;
    private Rigidbody grabbedRb;
    private bool isBlocked = false; // ���� �����ִ��� ����
    private string originalTag;  // ���� �±� ����
    private Collider grabbedCollider;
    private Collider playerCollider;

    void Start()
    {
        // �÷��̾��� Collider �������� (�ڵ� Ž��)
        playerCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryGrabObject();
        }

        if (Input.GetMouseButtonUp(0))
        {
            ReleaseObject();
        }

        if (grabbedObject && !isBlocked) // ���� ������ �ʾ��� ���� �̵�
        {
            MoveObjectWithPlayer();
        }
    }

    void TryGrabObject()
    {
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

                // **�±� ���� (���� �±� ����)**
                originalTag = grabbedObject.tag;
                grabbedObject.tag = "Untagged";  // �ӽ� �±׷� ����

                return; // ���� ����� ������Ʈ �ϳ��� ���
            }
        }
    }
    void MoveObjectWithPlayer()
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

    void ReleaseObject()
    {
        if (grabbedRb)
        {
            grabbedRb.useGravity = true;
            grabbedRb.freezeRotation = false;
            grabbedRb.isKinematic = false;
        }

        // **�±� ����**
        if (grabbedObject)
        {
            grabbedObject.tag = originalTag;
        }

        grabbedObject = null;
        grabbedRb = null;
        isBlocked = false; // �ʱ�ȭ
    }

    // ���� ������ �̵� ����
    /*public void SetBlocked(bool blocked)
    {
        isBlocked = blocked;
    }*/
}
