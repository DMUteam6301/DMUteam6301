using UnityEngine;

public class ObjectCollisionHandler : MonoBehaviour
{
    private PlayerObjectGrabber grabber;

    public void SetGrabber(PlayerObjectGrabber grabber)
    {
        this.grabber = grabber;
    }

   /* void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor")) // ���� ������ �̵� ����
        {
            grabber.SetBlocked(true);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor")) // ������ �������� �̵� ����
        {
            grabber.SetBlocked(false);
        }
    }*/
}
