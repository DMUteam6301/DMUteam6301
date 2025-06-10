using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Interact : MonoBehaviour
{
    public string targetTag = "Interactable"; // ��ȣ�ۿ��� ��� �±�
    public float interactRange = 2f; // ��ȣ�ۿ� �Ÿ�

    private Camera Cam;
    private CharacterController controller;

    [SerializeField] private AudioClip paperPickupSfx;
    [SerializeField] private AudioSource audioSource;

    void Start()
    {
        Cam = Camera.main;
        controller = GetComponent<CharacterController>();
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, interactRange))
            {
                if (hit.collider.CompareTag(targetTag))
                {
                    Interaction(hit.collider.gameObject);
                }
            }
        }
    }

    void Interaction(GameObject target)
    {
        Debug.Log("��ȣ�ۿ� ���: " + target.name);

        if (target.name == "eggpaper")
        {
            Debug.Log("ȸ���: " + target.name);
            audioSource.PlayOneShot(paperPickupSfx);
            target.SetActive(false);
        }
        else if (target.name == "sideroompaper")
        {
            Debug.Log("Ż�� �� ����: " + target.name);
            target.SetActive(false);
        }
        else if (target.name == "Paper")
        {
            Debug.Log("Ż�� �� ����: " + target.name);
            audioSource.PlayOneShot(paperPickupSfx);
            target.SetActive(false);
        }
    }

}
