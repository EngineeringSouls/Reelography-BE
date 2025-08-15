namespace Reelography.Service.Helper;

public static class StateToken
{
    public static string Create(string secret, int userId, TimeSpan ttl)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var exp = now + (long)ttl.TotalSeconds;
        var nonce = Guid.NewGuid().ToString("N");
        var payload = $"{userId}.{exp}.{nonce}";
        var sig = Sign(secret, payload);
        return Base64Url($"{payload}.{sig}");
    }

    public static (bool ok, int userId) Validate(string secret, string state)
    {
        try
        {
            var raw = FromBase64Url(state);
            var parts = raw.Split('.', 4);
            if (parts.Length != 4) return (false, 0);
            var userId = int.Parse(parts[0]);
            var exp = long.Parse(parts[1]);
            var payload = $"{parts[0]}.{parts[1]}.{parts[2]}";
            var sig = parts[3];
            if (sig != Sign(secret, payload)) return (false, 0);
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > exp) return (false, 0);
            return (true, userId);
        }
        catch { return (false, 0); }
    }

    private static string Sign(string secret, string data)
    {
        using var h = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret));
        return Convert.ToBase64String(h.ComputeHash(System.Text.Encoding.UTF8.GetBytes(data)));
    }
    private static string Base64Url(string s) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(s))
        .TrimEnd('=').Replace('+','-').Replace('/','_');
    private static string FromBase64Url(string s)
    {
        s = s.Replace('-', '+').Replace('_','/');
        switch (s.Length % 4){ case 2: s += "=="; break; case 3: s += "="; break; }
        var bytes = Convert.FromBase64String(s);
        return System.Text.Encoding.UTF8.GetString(bytes);
    }
}
