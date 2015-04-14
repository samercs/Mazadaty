
namespace Mzayad.Core.Formatting
{
    public static class PhoneNumberFormatter
    {
        public static string Format(string countryCode, string phoneNumber)
        {
            if (!countryCode.StartsWith("+"))
            {
                countryCode = "+" + countryCode;
            }
            
            return string.Format("{0} {1}", countryCode, phoneNumber);
        }
    }
}
