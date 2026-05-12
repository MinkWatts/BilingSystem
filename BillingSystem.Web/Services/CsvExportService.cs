using BillingSystem.Models.ViewModels;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace BillingSystem.Web.Services
{
    /// <summary>
    /// Feature 5: CSV Export using CsvHelper.
    /// </summary>
    public class CsvExportService
    {
        /// <summary>
        /// Exports a list of bills to a CSV byte array.
        /// </summary>
        public byte[] ExportBillsToCsv(List<RecentBillViewModel> bills)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };

            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms, System.Text.Encoding.UTF8);
            using var csv = new CsvWriter(writer, config);

            // Write header
            csv.WriteHeader<BillCsvRecord>();
            csv.NextRecord();

            // Write rows
            int sr = 1;
            foreach (var bill in bills)
            {
                csv.WriteRecord(new BillCsvRecord
                {
                    SrNo = sr++,
                    BillId = bill.Id,
                    InvoiceNumber = bill.InvoiceNumber,
                    AgentName = bill.AgentName,
                    TotalAmount = bill.TotalAmount,
                    Date = bill.CreatedDate.ToString("dd MMM yyyy")
                });
                csv.NextRecord();
            }

            writer.Flush();
            return ms.ToArray();
        }
    }

    /// <summary>
    /// CSV record shape for bills export.
    /// </summary>
    public class BillCsvRecord
    {
        public int SrNo { get; set; }
        public int BillId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public string AgentName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Date { get; set; } = string.Empty;
    }
}
