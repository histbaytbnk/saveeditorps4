
// Type: Ionic.EnumUtil


// Hacked by SystemAce

using System;
using System.ComponentModel;

namespace Ionic
{
  internal sealed class EnumUtil
  {
    private EnumUtil()
    {
    }

    internal static string GetDescription(Enum value)
    {
      DescriptionAttribute[] descriptionAttributeArray = (DescriptionAttribute[]) value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof (DescriptionAttribute), false);
      if (descriptionAttributeArray.Length > 0)
        return descriptionAttributeArray[0].Description;
      return value.ToString();
    }

    internal static object Parse(Type enumType, string stringRepresentation)
    {
      return EnumUtil.Parse(enumType, stringRepresentation, false);
    }

    internal static object Parse(Type enumType, string stringRepresentation, bool ignoreCase)
    {
      if (ignoreCase)
        stringRepresentation = stringRepresentation.ToLower();
      foreach (Enum @enum in Enum.GetValues(enumType))
      {
        string str = EnumUtil.GetDescription(@enum);
        if (ignoreCase)
          str = str.ToLower();
        if (str == stringRepresentation)
          return (object) @enum;
      }
      return Enum.Parse(enumType, stringRepresentation, ignoreCase);
    }
  }
}
