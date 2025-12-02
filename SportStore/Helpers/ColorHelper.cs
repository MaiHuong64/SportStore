namespace SportStore.Helpers;

public static class ColorHelper
{
    private static readonly Dictionary<string, string> ColorMap = new(StringComparer.OrdinalIgnoreCase)
    {
        {"Đen", "#000000"}, {"Black", "#000000"},
        {"Trắng", "#FFFFFF"}, {"White", "#FFFFFF"},
        {"Đỏ", "#FF0000"}, {"Red", "#FF0000"},
        {"Xanh dương", "#0000FF"}, {"Blue", "#0000FF"},
        {"Xanh lá", "#00FF00"}, {"Green", "#00FF00"},
        {"Vàng", "#FFFF00"}, {"Yellow", "#FFFF00"},
        {"Cam", "#FFA500"}, {"Orange", "#FFA500"},
        {"Hồng", "#FFC0CB"}, {"Pink", "#FFC0CB"},
        {"Tím", "#800080"}, {"Purple", "#800080"},
        {"Nâu", "#8B4513"}, {"Brown", "#8B4513"},
        {"Xám", "#808080"}, {"Gray", "#808080"}, {"Grey", "#808080"}
    };

    public static string GetHexColor(string? colorName)
    {
        if (string.IsNullOrEmpty(colorName))
            return "#CCCCCC";

        if (colorName.StartsWith("#"))
            return colorName;

        return ColorMap.TryGetValue(colorName, out var hexColor) ? hexColor : "#CCCCCC";
    }
}
