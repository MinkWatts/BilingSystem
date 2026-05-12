using QRCoder;

namespace BillingSystem.Web.Services
{
    /// <summary>
    /// Feature 2: QR Code generation using QRCoder (PNG-based, no GDI+ dependency).
    /// </summary>
    public static class QrCodeService
    {
        /// <summary>
        /// Generates a QR code as a PNG byte array.
        /// Uses PngByteQRCode which has no System.Drawing dependency.
        /// Returns null on failure.
        /// </summary>
        public static byte[]? GenerateQrCodeBytes(string content, int pixelsPerModule = 10)
        {
            if (string.IsNullOrWhiteSpace(content))
                return null;

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(pixelsPerModule);
        }

        /// <summary>
        /// Generates a QR code as a Base64 data URI for embedding in HTML/Razor views.
        /// Returns null on failure.
        /// </summary>
        public static string? GenerateQrCodeBase64(string content, int pixelsPerModule = 5)
        {
            var bytes = GenerateQrCodeBytes(content, pixelsPerModule);
            if (bytes == null || bytes.Length == 0)
                return null;
            return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
        }

        /// <summary>
        /// Builds the standard QR content string for an invoice.
        /// </summary>
        public static string BuildInvoiceQrContent(
            string invoiceNumber,
            string customerName,
            decimal grandTotal,
            DateTime generatedAt)
            => $"Invoice:{invoiceNumber}|Customer:{customerName}|Total:{grandTotal:N2}|Date:{generatedAt:yyyy-MM-dd}";
    }
}
