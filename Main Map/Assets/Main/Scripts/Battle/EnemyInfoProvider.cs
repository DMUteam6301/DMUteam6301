using UnityEngine;

public class EnemyInfoProvider : MonoBehaviour
{
    public static EnemyInfoProvider Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public EnemyInfo GenerateInfo()
    {
        return new EnemyInfo
        {
            enemyName = "Valkyrie-SX",
            type = "�κ�",
            weakness = "���� ȸ��",
            mapHint = "���� ���⿡ Ż�ⱸ ����"
        };
    }

    public string FormatInfoText(EnemyInfo info, bool detailed)
    {
        if (!detailed)
            return "<color=yellow>�Ϻ� ������ ���ŵǾ����ϴ� (��ŷ ����)</color>";

        return $"<b>�� ��ŷ ����</b>\n" +
               $"- �̸�: <color=cyan>{info.enemyName}</color>\n" +
               $"- ����: <color=orange>{info.type}</color>\n" +
               $"- ����: <color=red>{info.weakness}</color>\n" +
               $"- �� ����: <color=yellow>{info.mapHint}</color>";
    }
}
