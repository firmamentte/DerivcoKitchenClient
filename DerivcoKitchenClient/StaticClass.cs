using System.ComponentModel;

namespace DerivcoKitchenClient
{
    public static class StaticClass
    {
        public static class EnumHelper
        {
            public enum CurrencyCode
            {
                [Description("ZAR")]
                Code
            }

            public static string GetEnumDescription(Enum enumValue)
            {
                return FirmamentUtilities.Utilities.GetEnumDescription(enumValue);
            }

            public enum MessageSymbol
            {
                [Description("i")]
                Information,
                [Description("x")]
                Error
            }
        }
    }
}
