namespace Application.Services;

public static class MuscleGroupNormalizer
{
    // Basic canonical mapping; can be extended or moved to configuration later.
    private static readonly Dictionary<string,string> _map = new(StringComparer.OrdinalIgnoreCase)
    {
        {"chest","Chest"}, {"pectorals","Chest"}, {"pecs","Chest"},
        {"back","Back"}, {"lats","Back"},
        {"legs","Legs"}, {"quads","Legs"}, {"hamstrings","Legs"}, {"calves","Legs"},
        {"arms","Arms"}, {"biceps","Arms"}, {"triceps","Arms"}, {"forearms","Arms"},
        {"shoulders","Shoulders"}, {"delts","Shoulders"}, {"deltoids","Shoulders"},
        {"core","Core"}, {"abs","Core"}, {"abdominals","Core"}
    };

    public static string Normalize(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return "Other";
        var key = raw.Trim();
        return _map.TryGetValue(key, out var canonical) ? canonical : Capitalize(key);
    }

    private static string Capitalize(string s)
    {
        if (s.Length == 0) return s;
        return char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();
    }
}
