using UnityEngine;

public static class TimeConverter
{
    /// <summary>
    /// 10000000에서 입력된 밀리초를 뺀 값을 분:초.밀리초 형식으로 변환
    /// </summary>
    /// <param name="milliseconds">밀리초 값</param>
    /// <returns>분:초.밀리초 형식의 문자열</returns>
    public static string ConvertMillisecondsToTime(long milliseconds)
    {
        long adjustedMilliseconds = 10000000 - milliseconds; // 10000000에서 입력값을 뺌
        if (adjustedMilliseconds < 0) adjustedMilliseconds = 0; // 음수가 되지 않도록 보정

        long totalSeconds = adjustedMilliseconds / 1000; // 총 초
        long minutes = totalSeconds / 60;                // 분
        long seconds = totalSeconds % 60;                // 초
        long millis = adjustedMilliseconds % 1000;       // 남은 밀리초

        return $"{minutes:D2}:{seconds:D2}.{millis:D3}";
    }
}
