using MobileApped.Core.Persistence.Sqlite.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace MobileApped.Core.Persistence.Sqlite
{
    public static class Extensions
    {
        public static string GetEnumMemberValue<TEnum>(this TEnum value)
            where TEnum : Enum
        {
            var enumType = typeof(PragmaName);
            var member = enumType.GetMember(Enum.GetName(typeof(TEnum), value))?.FirstOrDefault();

            EnumMemberAttribute attribute = member?.GetCustomAttributes(
                    typeof(EnumMemberAttribute), true).FirstOrDefault()
                    as EnumMemberAttribute;
            return attribute?.Value;
        }
    }
}
