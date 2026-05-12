namespace BillingSystem.Models.Helpers
{
    /// <summary>
    /// Feature 3: Multi-currency support helper.
    /// Provides currency symbols and a static exchange rate table.
    /// </summary>
    public static class CurrencyHelper
    {
        // Supported currencies with their symbols
        public static readonly Dictionary<string, string> Currencies = new()
        {
            { "INR", "₹" },
            { "USD", "$" },
            { "EUR", "€" },
            { "GBP", "£" },
            { "AED", "د.إ" },
            { "JPY", "¥" },
            { "CAD", "CA$" },
            { "AUD", "A$" }
        };

        // Static exchange rates relative to INR (base currency).
        private static readonly Dictionary<string, decimal> RatesFromINR = new()
        {
            { "INR", 1m },
            { "USD", 0.012m },
            { "EUR", 0.011m },
            { "GBP", 0.0095m },
            { "AED", 0.044m },
            { "JPY", 1.80m },
            { "CAD", 0.016m },
            { "AUD", 0.018m }
        };

        public static string GetSymbol(string currencyCode)
            => Currencies.TryGetValue(currencyCode, out var sym) ? sym : currencyCode;

        public static decimal GetExchangeRate(string currencyCode)
            => RatesFromINR.TryGetValue(currencyCode, out var rate) ? rate : 1m;

        public static decimal Convert(decimal inrAmount, string targetCurrency)
            => inrAmount * GetExchangeRate(targetCurrency);
    }
}
