using UnityEngine;

public class ObjectGrabber : MonoBehaviour
{
    public Camera cam;
    public float grabDistance = 5f;
    public float moveSpeed = 10f;

    private GameObject grabOb;
    private Rigidbody rigid;
    private Vector3 Vectarget;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���콺 ���� Ŭ��
        {
            TryGrabObject();
        }

        if (Input.GetMouseButtonUp(0)) // ���콺 ��ư���� ���� ���� ����
        {
            ReleaseObject();
        }

        if (grabOb)
        {
            MoveObject();
        }
    }

    void TryGrabObject()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, grabDistance))
        {
            if (hit.collider.CompareTag("Grabbable"))  // "Grabbable" �±װ� �ִ� ������Ʈ�� ��� ����
            {
                grabOb = hit.collider.gameObject;
                rigid = grabOb.GetComponent<Rigidbody>();

                if (rigid)
                {
                    rigid.useGravity = false;  // �߷� ����
                    rigid.freezeRotation = true;  // ȸ�� ����
                    rigid.isKinematic = true;  // Kinematic���� ���� (velocity ���� ����)
                }
            }
        }
    }

    void MoveObject()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Vectarget = ray.origin + ray.direction * grabDistance;

        if (grabOb)
        {
            grabOb.transform.position = Vector3.Lerp(grabOb.transform.position, Vectarget, Time.deltaTime * moveSpeed);
        }
    }

    void ReleaseObject()
    {
        if (rigid)
        {
            rigid.useGravity = true;
            rigid.freezeRotation = false;
            rigid.isKinematic = false;  // �ٽ� ���� ����
        }

        grabOb = null;
        rigid = null;
    }
}
