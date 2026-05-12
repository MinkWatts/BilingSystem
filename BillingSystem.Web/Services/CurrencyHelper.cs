using BillingSystem.Models.Helpers;

namespace BillingSystem.Web.Services
{
    /// <summary>
    /// Feature 3: Multi-currency support helper - re-exports from Models for Web layer convenience.
    /// </summary>
    public static class CurrencyHelperWeb
    {
        public static string GetSymbol(string currencyCode) => CurrencyHelper.GetSymbol(currencyCode);
        public static decimal GetExchangeRate(string currencyCode) => CurrencyHelper.GetExchangeRate(currencyCode);
        public static Dictionary<string, string> Currencies => CurrencyHelper.Currencies;
    }
}
