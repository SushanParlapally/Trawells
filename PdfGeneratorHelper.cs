using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using TravelDesk.Models;
using System.IO;

namespace TravelDesk.Controllers
{
    public static class PdfGeneratorHelper
    {
        /// <summary>
        /// Generates a PDF document for a travel request using itext7
        /// </summary>
        public static byte[] GenerateTravelRequestPdf(TravelRequest travelRequest)
        {
            using (var stream = new MemoryStream())
            {
                // Create PDF writer and document using itext7
                using (var writer = new PdfWriter(stream))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        var document = new Document(pdf);
                        
                        // Add title
                        var title = new Paragraph("Travel Request Confirmation")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(18)
                            .SetBold()
                            .SetMarginBottom(20);
                        document.Add(title);

                        // Create travel details table
                        var travelTable = new Table(2)
                            .SetWidth(UnitValue.CreatePercentValue(100))
                            .SetMarginBottom(20);

                        // Add travel details
                        travelTable.AddCell(CreateCell("From Date:", true));
                        travelTable.AddCell(CreateCell(travelRequest.FromDate.ToString("yyyy-MM-dd"), false));
                        
                        travelTable.AddCell(CreateCell("To Date:", true));
                        travelTable.AddCell(CreateCell(travelRequest.ToDate.ToString("yyyy-MM-dd"), false));
                        
                        travelTable.AddCell(CreateCell("From Location:", true));
                        travelTable.AddCell(CreateCell(travelRequest.FromLocation, false));
                        
                        travelTable.AddCell(CreateCell("To Location:", true));
                        travelTable.AddCell(CreateCell(travelRequest.ToLocation, false));
                        
                        travelTable.AddCell(CreateCell("Reason:", true));
                        travelTable.AddCell(CreateCell(travelRequest.ReasonForTravel, false));

                        document.Add(travelTable);

                        // Create user information table
                        var userTable = new Table(2)
                            .SetWidth(UnitValue.CreatePercentValue(100))
                            .SetMarginBottom(20);

                        userTable.AddCell(CreateCell("Traveler Name:", true));
                        userTable.AddCell(CreateCell($"{travelRequest.UserName?.FirstName ?? "Unknown"} {travelRequest.UserName?.LastName ?? "User"}", false));
                        
                        userTable.AddCell(CreateCell("Email:", true));
                        userTable.AddCell(CreateCell(travelRequest.UserName?.Email ?? "No email", false));
                        
                        userTable.AddCell(CreateCell("Project:", true));
                        userTable.AddCell(CreateCell(travelRequest.Project?.ProjectName ?? "No project", false));
                        
                        userTable.AddCell(CreateCell("Department:", true));
                        userTable.AddCell(CreateCell(travelRequest.Department?.DepartmentName ?? "No department", false));

                        document.Add(userTable);

                        // Add footer
                        var footer = new Paragraph($"Generated on: {DateTime.UtcNow :yyyy-MM-dd HH:mm:ss}")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(10)
                            .SetMarginTop(30);
                        document.Add(footer);

                        document.Close();
                    }
                }
                
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Helper method to create table cells with consistent styling for itext7
        /// </summary>
        private static Cell CreateCell(string text, bool isHeader)
        {
            var cell = new Cell().Add(new Paragraph(text));
            
            if (isHeader)
            {
                cell.SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetBold()
                    .SetFontSize(12);
            }
            else
            {
                cell.SetFontSize(11);
            }
            
            cell.SetPadding(8)
                .SetBorder(new SolidBorder(1));
                
            return cell;
        }
    }
}