using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueTimerController : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI timerText;
    public Button threatenButton;
    public Button persuadeButton;
    public Button ignoreButton;

    public float timeLimit = 15f; // ���� �ð� (��)

    private float remainingTime;
    private bool isChoiceMade = false;

    void Start()
    {
        // �ʱ�ȭ
        dialogueText.text = "����� � ���� �ǳڱ��?";
        remainingTime = timeLimit;

        // ��ư�� �Լ� ����
        threatenButton.onClick.AddListener(() => OnChoiceSelected("����"));
        persuadeButton.onClick.AddListener(() => OnChoiceSelected("ȸ��"));
        ignoreButton.onClick.AddListener(() => OnChoiceSelected("����"));

        StartCoroutine(StartTimer());
    }

    IEnumerator StartTimer()
    {
        while (remainingTime > 0 && !isChoiceMade)
        {
            remainingTime -= Time.deltaTime;
            timerText.text = $"���� �ð�: {Mathf.CeilToInt(remainingTime)}��";
            yield return null;
        }

        if (!isChoiceMade)
        {
            OnTimeout();
        }
    }

    void OnChoiceSelected(string choice)
    {
        isChoiceMade = true;
        Debug.Log($"�÷��̾ '{choice}' ������");

        // �̰����� ���ÿ� ���� ������ ����� ����
        ApplyDebuff(choice);

        // ���� ��Ʈ��ũ ���۵� ���⼭
        // NetworkManager.SendChoiceToOtherPlayer(choice);
    }

    void OnTimeout()
    {
        Debug.Log("�ð� �ʰ�! �������� ����. ����Ʈ ó��.");
        ApplyDefaultOutcome();
    }

    void ApplyDebuff(string choice)
    {
        // ����: �������� ���� ������ �ٸ� ����� ����
        switch (choice)
        {
            case "����":
                Debug.Log("�� �� ���ݷ� ��ȭ!");
                break;
            case "ȸ��":
                Debug.Log("�� ��ȭ������ ��ȯ");
                break;
            case "����":
                Debug.Log("�� �ƹ��ϵ� �������� �ʾҴ�");
                break;
        }

        // ���� UI �ݱ�, �� �ѱ�� �� ó��
    }

    void ApplyDefaultOutcome()
    {
        Debug.Log("�� ���� ������ ������ ����!");
        // ��: ���� ���� +10%
    }
}
