namespace SportStore.Helpers;

public static class SkuHelper
{
    public static string GenerateSku(string productCode, byte? size, string? color)
    {
        var sizePart = size.HasValue ? size.Value.ToString() : "NS";
        var colorPart = !string.IsNullOrEmpty(color) ? color.Replace(" ", "").Replace("#", "") : "NC";
        return $"{productCode}-{sizePart}-{colorPart}";
    }
}
