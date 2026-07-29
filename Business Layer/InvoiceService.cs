using SardarJi_Cab_Booking.Models;
using System.Reflection.Metadata;
using QuestPDF.Infrastructure;
using Document = QuestPDF.Fluent.Document;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace SardarJi_Cab_Booking.Business_Layer
{
    public class InvoiceService : IInvoiceService
    {
        static InvoiceService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GenerateInvoicePdf(BookingListItem b)
        {
            var document = QuestPDF.Fluent.Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("CAB BOOKING INVOICE")
                                .FontSize(20).Bold().FontColor(Colors.Blue.Darken2);
                            col.Item().Text($"Invoice #: INV-{b.BookingId:D6}").FontSize(9).FontColor(Colors.Grey.Darken1);
                            col.Item().Text($"Booking No: {b.BookingNo}").FontSize(9).FontColor(Colors.Grey.Darken1);
                        });

                        row.ConstantItem(150).Column(col =>
                        {
                            col.Item().AlignRight().Text("Sardar Ji Cab Pvt. Ltd.").Bold();
                            col.Item().AlignRight().Text("sardarjiweb.traviyo.in").FontSize(8);
                            col.Item().AlignRight().Text("support@sardarjiweb.com").FontSize(8);
                        });
                    });

                    page.Content().PaddingTop(20).Column(col =>
                    {
                        col.Spacing(15);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Billed To").Bold().FontSize(11);
                                c.Item().Text(b.PassengerName ?? "-");
                                c.Item().Text(b.ContactNumber ?? "-");
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().AlignRight().Text("Journey Date").Bold().FontSize(11);
                                c.Item().AlignRight().Text(b.JourneyDate.ToString("dd MMM yyyy, hh:mm tt"));
                                c.Item().AlignRight().Text($"Status: {b.Status}");
                            });
                        });

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().Text("Trip Details").Bold().FontSize(11);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn(1);
                                c.RelativeColumn(2);
                            });

                            void Row(string label, string value)
                            {
                                table.Cell().Padding(4).Text(label).FontColor(Colors.Grey.Darken1);
                                table.Cell().Padding(4).Text(value ?? "-");
                            }

                            Row("Route", b.CabRoute);
                            Row("Pickup Address", b.PickupAddress);
                            Row("Drop Address", b.DropAddress);
                            Row("Distance", b.TotalDistanceKm.HasValue ? $"{b.TotalDistanceKm} km" : "-");
                            Row("Vehicle", $"{b.VehicleName} ({b.VehicleModelYear}) - {b.VehicleColor}");
                            Row("Fuel Type", b.VehicleFuelType);
                            Row("Payment Method","Online");
                        });

                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                        col.Item().AlignRight().Column(c =>
                        {
                            c.Item().Text("₹ Total Fare: {b.TotalFare:C2}").FontSize(11);
                            c.Item().Text("₹ Net Payable: {b.NetPayable:C2}").Bold().FontSize(13).FontColor(Colors.Green.Darken2);
                        });

                        if (!string.IsNullOrWhiteSpace(b.BarcodeInfo))
                        {
                            col.Item().AlignCenter().PaddingTop(10).Text(b.BarcodeInfo).FontSize(8).FontColor(Colors.Grey.Medium);
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Generated on ").FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(DateTime.Now.ToString("dd MMM yyyy hh:mm tt")).FontSize(8).FontColor(Colors.Grey.Medium);
                        x.Span(" — This is a system generated invoice.").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
