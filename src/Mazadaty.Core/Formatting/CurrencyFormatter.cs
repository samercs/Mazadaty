using System;

namespace Mazadaty.Core.Formatting
{
    public enum Currency
    {
        Usd = 1,
        Kwd,
        Gbp,
        Eur,
        Sar,
        Bhd,
        Qar,
        Aed,
        Omr
    }
    
    public class CurrencyFormatter
    {   
        public static string Format(decimal? amount, Currency currency = Currency.Kwd)
        {
            if (!amount.HasValue)
            {
                return "";
            }
            
            switch (currency)
            {
                case Currency.Kwd: return "KD " + GetFormattedAmount(amount.Value, 3);
                case Currency.Usd: return "$" + GetFormattedAmount(amount.Value);
                case Currency.Gbp: return "&pound;" + GetFormattedAmount(amount.Value);
                case Currency.Eur: return "&euro;" + GetFormattedAmount(amount.Value);
                case Currency.Sar: return "SR " + GetFormattedAmount(amount.Value);
                case Currency.Bhd: return "BD " + GetFormattedAmount(amount.Value, 3);
                case Currency.Qar: return "QR " + GetFormattedAmount(amount.Value);
                case Currency.Aed: return "DH " + GetFormattedAmount(amount.Value);
                case Currency.Omr: return "OMR " + GetFormattedAmount(amount.Value, 3);
                
                default: throw new NotImplementedException();
            }
        }

        private static string GetFormattedAmount(decimal amount, int? decimalPlaces = 2)
        {
            if (amount % 1 == 0)
            {
                return string.Format("{0:#,0}", Math.Floor(amount));
            }

            switch (decimalPlaces)
            {
                case 2: return string.Format("{0:#,0.00}", amount);
                case 3: return string.Format("{0:#,0.000}", amount);
                default: throw new NotImplementedException();
            }
        }
    }
}
