using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
namespace Domain.Enums;

public static class EnumExtensions
{
    public static string GetDisplayAttributeValue(this Enum enumValue)
    {
        return enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DisplayAttribute>()?.GetName() ?? enumValue.ToString();
    }
    public static string GetDescriptionAttributeValue(this Enum enumValue)
    {
        return enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DescriptionAttribute>()?.Description ?? enumValue.ToString();
    }
    public static string GetEnumName(this Enum enumValue)
    {
        return enumValue.ToString();
    }
}
