using System.ComponentModel;

namespace SportHive.Implementations
{
    public class EnumWork
    {
        public static bool TryParseStyleFromText<TEnum>(string input, out int numericValue) where TEnum : Enum
        {
            foreach (var field in typeof(TEnum).GetFields())
            {
                var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attr != null && attr.Description.Equals(input, StringComparison.OrdinalIgnoreCase))
                {
                    var enumValue = (TEnum)field.GetValue(null);
                    numericValue = Convert.ToInt32(enumValue);
                    return true;
                }
            }

            numericValue = default;
            return false;
        }



        public static string GetDescription<TEnum>(TEnum style)
        {
            var field = typeof(TEnum).GetField(style.ToString());
            var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attr?.Description ?? style.ToString();
        }

    }
}