namespace Reelography.Shared.StaticHelpers;

public static class NotificationHelper
{
    private static readonly Random Random = new Random();

    public static string GenerateOtp(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[Random.Next(s.Length)]).ToArray());
    }

    public static string GenerateBodyWithPlaceholder(Dictionary<string, string> placeholders, string templateText)
    {
        foreach (var kv in placeholders)
        {
            templateText = templateText.Replace("{{" + kv.Key + "}}", kv.Value);
        }
        return templateText;
    }
}