using UnityEngine;

public static class TimeConverter
{
    /// <summary>
    /// 10000000���� �Էµ� �и��ʸ� �� ���� ��:��.�и��� �������� ��ȯ
    /// </summary>
    /// <param name="milliseconds">�и��� ��</param>
    /// <returns>��:��.�и��� ������ ���ڿ�</returns>
    public static string ConvertMillisecondsToTime(long milliseconds)
    {
        long adjustedMilliseconds = 10000000 - milliseconds; // 10000000���� �Է°��� ��
        if (adjustedMilliseconds < 0) adjustedMilliseconds = 0; // ������ ���� �ʵ��� ����

        long totalSeconds = adjustedMilliseconds / 1000; // �� ��
        long minutes = totalSeconds / 60;                // ��
        long seconds = totalSeconds % 60;                // ��
        long millis = adjustedMilliseconds % 1000;       // ���� �и���

        return $"{minutes:D2}:{seconds:D2}.{millis:D3}";
    }
}
