using System.ComponentModel;
using System.Reflection;

namespace Reelography.Shared.Extensions;

public static class EnumExtension
{
    /// <summary>
    /// Get the description 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string GetDescription(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name == null) return string.Empty;

        var field = type.GetField(name);
        if (field == null) return string.Empty;

        var attr = field.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? name;
    }
}