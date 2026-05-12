using BillingSystem.Models.DTOs;
using BillingSystem.Models.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BillingSystem.Web.Services
{
    /// <summary>
    /// Feature 1: PDF Invoice Generation using QuestPDF.
    /// </summary>
    public class InvoicePdfService
    {
        public byte[] GenerateInvoicePdf(InvoiceDto invoice)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    // ── HEADER ──
                    page.Header().Element(ComposeHeader);

                    // ── CONTENT ──
                    page.Content().Element(content =>
                    {
                        content.Column(col =>
                        {
                            col.Spacing(12);

                            // Invoice meta + customer info
                            col.Item().Row(row =>
                            {
                                // Left: Invoice details
                                row.RelativeItem().Column(left =>
                                {
                                    left.Item().Text("INVOICE").FontSize(22).Bold().FontColor("#1a56db");
                                    left.Item().Text($"# {invoice.InvoiceNumber}").FontSize(13).Bold();
                                    left.Item().Text($"Date: {invoice.GeneratedAt:dd MMM yyyy}").FontColor("#64748b");
                                    left.Item().Text($"Agent: {invoice.AgentName}").FontColor("#64748b");
                                });

                                // Right: Customer details
                                row.RelativeItem().Column(right =>
                                {
                                    right.Item().Text("BILL TO").Bold().FontColor("#94a3b8").FontSize(9);
                                    right.Item().Text(invoice.CustomerName).FontSize(13).Bold();
                                    if (!string.IsNullOrEmpty(invoice.CustomerContact))
                                        right.Item().Text($"📞 {invoice.CustomerContact}").FontColor("#64748b");
                                    if (!string.IsNullOrEmpty(invoice.Remarks))
                                        right.Item().Text($"Note: {invoice.Remarks}").Italic().FontColor("#94a3b8");
                                });
                            });

                            // Divider
                            col.Item().LineHorizontal(1).LineColor("#e2e8f0");

                            // Items table
                            col.Item().Element(ComposeItemsTable);

                            // Totals
                            col.Item().Element(ComposeTotals);

                            // QR Code section
                            col.Item().Element(ComposeQrSection);
                        });
                    });

                    // ── FOOTER ──
                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("Thank you for your business! | Billing System | ").FontColor("#94a3b8");
                        text.Span("Page ").FontColor("#94a3b8");
                        text.CurrentPageNumber().FontColor("#94a3b8");
                        text.Span(" of ").FontColor("#94a3b8");
                        text.TotalPages().FontColor("#94a3b8");
                    });

                    // Local helpers
                    void ComposeHeader(IContainer c)
                    {
                        c.Row(row =>
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text("BILLING SYSTEM").FontSize(20).Bold().FontColor("#0f172a");
                                col.Item().Text("Official Tax Invoice").FontSize(9).FontColor("#94a3b8");
                            });
                            row.ConstantItem(120).AlignRight().Column(col =>
                            {
                                col.Item().Background("#1a56db").Padding(6)
                                    .Text("INVOICE").FontColor(Colors.White).Bold().FontSize(11).AlignCenter();
                            });
                        });
                    }

                    void ComposeItemsTable(IContainer c)
                    {
                        var sym = invoice.CurrencySymbol;
                        var rate = invoice.ExchangeRate;

                        c.Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.ConstantColumn(30);   // #
                                cols.RelativeColumn(4);    // Product
                                cols.RelativeColumn(1.5f); // Qty
                                cols.RelativeColumn(2);    // Unit Price
                                cols.RelativeColumn(2);    // Total
                            });

                            // Header row
                            static IContainer HeaderCell(IContainer c) =>
                                c.Background("#0f172a").Padding(8);

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderCell).Text("#").FontColor(Colors.White).Bold();
                                header.Cell().Element(HeaderCell).Text("Product").FontColor(Colors.White).Bold();
                                header.Cell().Element(HeaderCell).AlignCenter().Text("Qty").FontColor(Colors.White).Bold();
                                header.Cell().Element(HeaderCell).AlignRight().Text("Unit Price").FontColor(Colors.White).Bold();
                                header.Cell().Element(HeaderCell).AlignRight().Text("Total").FontColor(Colors.White).Bold();
                            });

                            // Data rows
                            for (int i = 0; i < invoice.Items.Count; i++)
                            {
                                var item = invoice.Items[i];
                                var bgColor = i % 2 == 0 ? "#ffffff" : "#f8fafc";

                                IContainer DataCell(IContainer c) =>
                                    c.Background(bgColor).BorderBottom(1).BorderColor("#f1f5f9").Padding(8);

                                table.Cell().Element(DataCell).Text($"{i + 1}").FontColor("#94a3b8");
                                table.Cell().Element(DataCell).Text(item.ProductName).Bold();
                                table.Cell().Element(DataCell).AlignCenter().Text($"{item.Quantity}");
                                table.Cell().Element(DataCell).AlignRight()
                                    .Text($"{sym}{(item.UnitPrice * rate):N2}");
                                table.Cell().Element(DataCell).AlignRight()
                                    .Text($"{sym}{(item.LineTotal * rate):N2}").Bold();
                            }
                        });
                    }

                    void ComposeTotals(IContainer c)
                    {
                        var sym = invoice.CurrencySymbol;
                        var rate = invoice.ExchangeRate;

                        var subTotal = invoice.Items.Sum(x => x.LineTotal) * rate;
                        var discountAmt = subTotal * (invoice.DiscountPercent / 100m);
                        var afterDiscount = subTotal - discountAmt;
                        var taxAmt = afterDiscount * (invoice.TaxPercent / 100m);
                        var grandTotal = afterDiscount + taxAmt;

                        c.AlignRight().Width(280).Column(col =>
                        {
                            col.Item().Row(r =>
                            {
                                r.RelativeItem().Text("Sub Total").FontColor("#64748b");
                                r.ConstantItem(120).AlignRight().Text($"{sym}{subTotal:N2}");
                            });

                            if (invoice.DiscountPercent > 0)
                            {
                                col.Item().Row(r =>
                                {
                                    r.RelativeItem().Text($"Discount ({invoice.DiscountPercent}%)").FontColor("#64748b");
                                    r.ConstantItem(120).AlignRight().Text($"- {sym}{discountAmt:N2}").FontColor("#e02424");
                                });
                            }

                            col.Item().Row(r =>
                            {
                                r.RelativeItem().Text($"Tax ({invoice.TaxPercent}%)").FontColor("#64748b");
                                r.ConstantItem(120).AlignRight().Text($"{sym}{taxAmt:N2}");
                            });

                            col.Item().LineHorizontal(1).LineColor("#e2e8f0");

                            col.Item().Background("#e8f0fe").Padding(8).Row(r =>
                            {
                                r.RelativeItem().Text("GRAND TOTAL").Bold().FontSize(12).FontColor("#1a56db");
                                r.ConstantItem(120).AlignRight().Text($"{sym}{grandTotal:N2}").Bold().FontSize(12).FontColor("#1a56db");
                            });

                            if (invoice.Currency != "INR")
                            {
                                col.Item().Text($"* Amounts shown in {invoice.Currency} (Rate: 1 INR = {rate} {invoice.Currency})")
                                    .FontSize(8).Italic().FontColor("#94a3b8");
                            }
                        });
                    }

                    void ComposeQrSection(IContainer c)
                    {
                        // Calculate actual grand total with discount & tax
                        var rate = invoice.ExchangeRate;
                        var subTotalForQr = invoice.Items.Sum(x => x.LineTotal) * rate;
                        var discountForQr = subTotalForQr * (invoice.DiscountPercent / 100m);
                        var afterDiscountForQr = subTotalForQr - discountForQr;
                        var taxForQr = afterDiscountForQr * (invoice.TaxPercent / 100m);
                        var grandTotalForQr = afterDiscountForQr + taxForQr;

                        // QR code data string
                        var qrData = $"Invoice:{invoice.InvoiceNumber}|Customer:{invoice.CustomerName}|Total:{grandTotalForQr:N2}|Date:{invoice.GeneratedAt:yyyy-MM-dd}";
                        var qrBytes = QrCodeService.GenerateQrCodeBytes(qrData);

                        c.Row(row =>
                        {
                            if (qrBytes != null && qrBytes.Length > 0)
                            {
                                row.ConstantItem(80).Column(col =>
                                {
                                    col.Item().Image(qrBytes).FitWidth();
                                    col.Item().Text("Scan to verify").FontSize(7).FontColor("#94a3b8").AlignCenter();
                                });
                            }
                            else
                            {
                                row.ConstantItem(80).Column(col =>
                                {
                                    col.Item().Border(1).BorderColor("#e2e8f0").Width(70).Height(70)
                                        .AlignCenter().AlignMiddle()
                                        .Text("QR N/A").FontSize(7).FontColor("#94a3b8");
                                });
                            }
                            row.RelativeItem().PaddingLeft(12).Column(col =>
                            {
                                col.Item().Text("Terms & Conditions").Bold().FontSize(9);
                                col.Item().Text("• Payment is due within 30 days of invoice date.").FontSize(8).FontColor("#64748b");
                                col.Item().Text("• This is a computer-generated invoice.").FontSize(8).FontColor("#64748b");
                                col.Item().Text("• For queries, contact support@billing-system.com").FontSize(8).FontColor("#64748b");
                            });
                        });
                    }
                });
            });

            return document.GeneratePdf();
        }
    }
}
