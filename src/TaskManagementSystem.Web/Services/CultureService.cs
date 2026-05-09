namespace TaskManagementSystem.Web.Services;

public static class CultureService
{
    public static string GetSwitchUrl(string targetCulture, string currentPath)
    {
        var path = string.IsNullOrEmpty(currentPath) ? "/" : $"/{currentPath.TrimStart('/')}";
        return $"/culture/set?culture={targetCulture}&redirectUri={Uri.EscapeDataString(path)}";
    }
}
