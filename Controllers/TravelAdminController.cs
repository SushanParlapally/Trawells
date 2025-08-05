using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using TravelDesk.Data;
using TravelDesk.Models;
using TravelDesk.DTO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Threading.Tasks;
using Document = iTextSharp.text.Document;
using TravelDesk.Service;
using TravelDesk.Interface;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using iTextSharp.text.pdf.draw;
using Microsoft.Extensions.Configuration;
namespace TravelDesk.Controllers
{
    [ApiController]
    [Route("api/travel-requests")]
    public class TravelAdminController : ControllerBase
    {
        private readonly TravelDeskContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public TravelAdminController(TravelDeskContext context, IEmailService emailService, IConfiguration configuration)
        {
            _context = context;
            _emailService = emailService;
            _configuration = configuration;
        }

        //        [HttpGet("GetAllRequests")]
        //        public async Task<IActionResult> GetAllRequests()
        //        {
        //            try
        //            {
        //                var travelRequests = await _context.TravelRequests
        //                    .Include(tr => tr.User) // Include the User details
        //                    .Select(tr => new
        //                    {
        //                        tr.TravelRequestId,
        //                        tr.Status,
        //                        tr.Comments,
        //                        tr.FromDate,
        //                        tr.ToDate,
        //                        tr.ReasonForTravel,
        //                        tr.FromLocation,
        //                        tr.ToLocation, 
        //                        tr.TicketUrl,

        //                        User = new
        //                        {
        //                            tr.User.UserId,
        //                            tr.User.FirstName,
        //                            tr.User.LastName,
        //                            tr.User.Email
        //                        },
        //                        Project = new
        //                        {
        //                            tr.Project.ProjectId,
        //                            tr.Project.ProjectName
        //                        },
        //                        Department = new
        //                        {
        //                            tr.Department.DepartmentId,
        //                            tr.Department.DepartmentName
        //                        }
        //                    })
        //                    .ToListAsync();

        //                if (travelRequests == null || travelRequests.Count == 0)
        //                {
        //                    return NotFound("No travel requests found.");
        //                }

        //                return Ok(travelRequests);
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(500, $"Internal server error: {ex.Message}");
        //            }
        //        }



        //        [HttpPost("BookTicket/{travelRequestId}")]
        //        public async Task<IActionResult> BookTicket(int travelRequestId, [FromBody] BookingDetails bookingDetails)
        //        {
        //            try
        //            {
        //                var travelRequest = await _context.TravelRequests
        //                    .Include(tr => tr.User) // Include user details
        //                    .FirstOrDefaultAsync(tr => tr.TravelRequestId == travelRequestId);

        //                if (travelRequest == null)
        //                {
        //                    return NotFound("Travel request not found.");
        //                }

        //                // Update the travel request with booking details
        //                travelRequest.Status = "Booked";
        //                travelRequest.Comments = bookingDetails.Comments; // Update comments

        //                // Save the booking confirmation URL if available
        //                if (!string.IsNullOrEmpty(bookingDetails.TicketUrl))
        //                {
        //                    travelRequest.TicketUrl = bookingDetails.TicketUrl;
        //                }

        //                _context.TravelRequests.Update(travelRequest);
        //                await _context.SaveChangesAsync();

        //                // Notify the employee via the history update
        //                return Ok("Booking confirmed successfully.");
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(500, $"Internal server error: {ex.Message}");
        //            }
        //        }

        //        [HttpPost("ReturnToManager/{travelRequestId}")]
        //        public async Task<IActionResult> ReturnToManager(int travelRequestId, [FromBody] BookingDetails bookingDetails)
        //        {
        //            try
        //            {
        //                var travelRequest = await _context.TravelRequests
        //                    .FirstOrDefaultAsync(tr => tr.TravelRequestId == travelRequestId);

        //                if (travelRequest == null)
        //                {
        //                    return NotFound("Travel request not found.");
        //                }

        //                // Reassign request to manager
        //                travelRequest.Status = "Returned to Manager";
        //                travelRequest.Comments = bookingDetails.Comments;


        //                _context.TravelRequests.Update(travelRequest);
        //                await _context.SaveChangesAsync();

        //                return Ok("Request returned to manager successfully.");
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(500, $"Internal server error: {ex.Message}");
        //            }
        //        }

        //        [HttpPost("ReturnToEmployee/{travelRequestId}")]
        //        public async Task<IActionResult> ReturnToEmployee(int travelRequestId, [FromBody] BookingDetails bookingDetails)
        //        {
        //            try
        //            {
        //                var travelRequest = await _context.TravelRequests
        //                    .FirstOrDefaultAsync(tr => tr.TravelRequestId == travelRequestId);

        //                if (travelRequest == null)
        //                {
        //                    return NotFound("Travel request not found.");
        //                }

        //                // Reassign request to employee
        //                travelRequest.Status = "Returned to Employee";
        //                travelRequest.Comments = bookingDetails.Comments;


        //                _context.TravelRequests.Update(travelRequest);
        //                await _context.SaveChangesAsync();

        //                return Ok("Request returned to employee successfully.");
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(500, $"Internal server error: {ex.Message}");
        //            }
        //        }

        //        [HttpPost("CloseRequest/{travelRequestId}")]
        //        public async Task<IActionResult> CloseRequest(int travelRequestId, [FromBody] BookingDetails bookingDetails)
        //        {
        //            // Ensure that CommentsRequest class matches the expected JSON payload
        //            try
        //            {
        //                var travelRequest = await _context.TravelRequests
        //                    .FirstOrDefaultAsync(tr => tr.TravelRequestId == travelRequestId);

        //                if (travelRequest == null)
        //                {
        //                    return NotFound("Travel request not found.");
        //                }

        //                // Close the request with complete status
        //                travelRequest.Status = "Completed";
        //                //travelRequest.Comments = bookingDetails.Comments;


        //                _context.TravelRequests.Update(travelRequest);
        //                await _context.SaveChangesAsync();

        //                return Ok("Request closed successfully.");
        //            }
        //            catch (Exception ex)
        //            {
        //                return StatusCode(500, $"Internal server error: {ex.Message}");
        //            }
        //        }



        //    }
        //}

        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            try
            {
                var travelRequests = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .Include(tr => tr.Project)
                    .Include(tr => tr.Department)
                    .Select(tr => new
                    {
                        tr.TravelRequestId,
                        tr.Status,
                        tr.Comments,
                        tr.FromDate,
                        tr.ToDate,
                        tr.ReasonForTravel,
                        tr.FromLocation,
                        tr.ToLocation,
                        tr.TicketUrl,
                        User = new
                        {
                            tr.UserName.UserId,
                            tr.UserName.FirstName,
                            tr.UserName.LastName,
                            tr.UserName.Email
                        },
                        Project = new
                        {
                            tr.Project.ProjectId,
                            tr.Project.ProjectName
                        },
                        Department = new
                        {
                            tr.Department.DepartmentId,
                            tr.Department.DepartmentName
                        }
                    })
                    .ToListAsync();

                if (travelRequests == null || travelRequests.Count == 0)
                {
                    return NotFound("No travel requests found.");
                }

                return Ok(travelRequests);
            }
            catch (Exception ex)
            {
                // Log exception here if you have a logging mechanism
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Books a ticket for a specific travel request.
        /// </summary>
        [HttpPost("{travelRequestId}/book")]
        public async Task<IActionResult> BookTicket(int travelRequestId, [FromBody] BookingDetails bookingDetails)
        {
            try
            {
                var travelRequest = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .FirstOrDefaultAsync(tr => tr.TravelRequestId == travelRequestId);

                if (travelRequest == null)
                {
                    return NotFound("Travel request not found.");
                }

                travelRequest.Status = "Booked";
                travelRequest.Comments = bookingDetails.Comments;
                if (!string.IsNullOrEmpty(bookingDetails.TicketUrl))
                {
                    travelRequest.TicketUrl = bookingDetails.TicketUrl;
                }

                _context.TravelRequests.Update(travelRequest);
                await _context.SaveChangesAsync();

                // Prepare email details
                var emailSubject = "Your Ticket has been Booked!";
                
                // Get the base URL from configuration or use a fallback
                var baseUrl = _configuration["AppSettings:BaseUrl"] ?? 
                             _configuration["AppSettings:ApiBaseUrl"] ?? 
                             $"{Request.Scheme}://{Request.Host}";
                
                // Ensure the base URL doesn't end with a slash
                baseUrl = baseUrl.TrimEnd('/');
                
                // Generate the PDF download URL for this travel request (public endpoint)
                var pdfDownloadUrl = $"{baseUrl}/api/travel-requests/{travelRequestId}/download-ticket";
                
                // Generate PDF content for email attachment
                byte[] pdfBytes = null;
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        var pdfResult = await GeneratePdfDocument(travelRequest);
                        if (pdfResult is FileContentResult fileResult)
                        {
                            pdfBytes = fileResult.FileContents;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log PDF generation error but continue with email
                    Console.WriteLine($"PDF generation failed: {ex.Message}");
                }
                
                var emailBody = $@"
<div style='font-family: Arial, sans-serif; max-width: 650px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px; box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1); background-color: #f9fbfc;'>
<!-- Header Section -->
<div style='background: linear-gradient(135deg, #3498db, #2980b9); padding: 20px; border-radius: 10px 10px 0 0; color: white; text-align: center;'>
<h1 style='margin: 0; font-size: 24px;'>🎟️ Booking Confirmation</h1>
<p style='font-size: 16px; margin-top: 5px;'>Your ticket has been successfully booked!</p>
</div>
<!-- Booking Details -->
<div style='padding: 20px; color: #444; background-color: white; border-radius: 0 0 10px 10px;'>
<h2 style='font-size: 20px; color: #2c3e50; border-bottom: 1px solid #e0e0e0; padding-bottom: 10px;'>Your Travel Information</h2>
<p style='margin-bottom: 10px;'>
<strong style='color: #34495e;'>From:</strong> {travelRequest.FromLocation}
</p>
<p style='margin-bottom: 10px;'>
<strong style='color: #34495e;'>To:</strong> {travelRequest.ToLocation}
</p>
<p style='margin-bottom: 10px;'>
<strong style='color: #34495e;'>Date:</strong> {travelRequest.FromDate.ToShortDateString()}
</p>
<p style='margin-bottom: 10px;'>
<strong style='color: #34495e;'>Time:</strong> {travelRequest.ToDate}
</p>
<p style='margin-bottom: 10px;'>
<strong style='color: #34495e;'>Traveler Name:</strong> {travelRequest.UserName}
</p>
<p style='margin-bottom: 10px;'>
<strong style='color: #34495e;'>Reason for Travel:</strong> {travelRequest.ReasonForTravel}
</p>
<p style='margin-bottom: 10px;'>
<strong style='color: #34495e;'>Ticket Generation Date:</strong> {travelRequest.CreatedOn}
</p>
 
        <!-- Button Section -->
<div style='text-align: center; margin-top: 20px;'>
<a href='{pdfDownloadUrl}' download style='display: inline-block; background-color: #3498db; color: white; padding: 15px 25px; text-decoration: none; font-size: 18px; border-radius: 5px; box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);'>
                📝 Download Your Ticket
</a>
</div>
</div>
 
    <!-- Footer Section -->
<div style='text-align: center; padding-top: 30px; color: #7f8c8d; font-size: 12px;'>
<p>If you have any questions, feel free to <a href='mailto:support@yourcompany.com' style='color: #3498db;'>contact us</a>.</p>
<p>Thank you for choosing us for your travel needs!</p>
</div>
</div>";

                // Send email with PDF attachment if available
                if (pdfBytes != null)
                {
                    await _emailService.SendEmailWithAttachmentAsync(
                        travelRequest.UserName.Email, 
                        emailSubject, 
                        emailBody, 
                        pdfBytes, 
                        $"TravelTicket_{travelRequestId}.pdf"
                    );
                }
                else
                {
                    // Send email without attachment if PDF generation failed
                    await _emailService.SendEmailAsync(travelRequest.UserName.Email, emailSubject, emailBody);
                }

                return Ok("Booking confirmed successfully.");
            }
            catch (Exception ex)
            {
                // Log exception details
                Console.WriteLine($"Failed to send email: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }



        // Public endpoint for PDF downloads (no authentication required)
        [HttpGet("{travelRequestId}/download-ticket")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadTicketPdf(int travelRequestId)
        {
            try
            {
                // Fetch travel request along with related user, project, and department details
                var travelRequest = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .Include(tr => tr.Project)
                    .Include(tr => tr.Department)
                    .FirstOrDefaultAsync(tr => tr.TravelRequestId == travelRequestId);

                // Check if travel request exists
                if (travelRequest == null)
                {
                    return NotFound("Travel request not found.");
                }

                // Check if the request is booked (only booked tickets should be downloadable)
                if (travelRequest.Status != "Booked")
                {
                    return BadRequest("Ticket is not yet booked. Only booked tickets can be downloaded.");
                }

                // Generate the PDF (same logic as the authenticated endpoint)
                return await GeneratePdfDocument(travelRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a secure download link for a ticket (with expiration)
        /// </summary>
        [HttpGet("{travelRequestId}/secure-download-link")]
        public async Task<IActionResult> GenerateSecureDownloadLink(int travelRequestId)
        {
            try
            {
                var travelRequest = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .FirstOrDefaultAsync(tr => tr.TravelRequestId == travelRequestId);

                if (travelRequest == null)
                {
                    return NotFound("Travel request not found.");
                }

                if (travelRequest.Status != "Booked")
                {
                    return BadRequest("Ticket is not yet booked. Only booked tickets can be downloaded.");
                }

                // Generate a secure token (you can implement a more sophisticated token system)
                var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"ticket_{travelRequestId}_{DateTime.UtcNow.Ticks}"));
                
                // Get the base URL from configuration
                var baseUrl = _configuration["AppSettings:BaseUrl"] ?? 
                             _configuration["AppSettings:ApiBaseUrl"] ?? 
                             $"{Request.Scheme}://{Request.Host}";
                
                baseUrl = baseUrl.TrimEnd('/');
                
                // Create secure download link
                var secureDownloadUrl = $"{baseUrl}/api/travel-requests/{travelRequestId}/download-ticket?token={token}";
                
                return Ok(new { 
                    downloadUrl = secureDownloadUrl,
                    expiresAt = DateTime.UtcNow.AddHours(24), // Link expires in 24 hours
                    message = "Secure download link generated successfully"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{travelRequestId}/ticket-pdf")]
        public async Task<IActionResult> GenerateTicketPdf(int travelRequestId)
        {
            try
            {
                // Fetch travel request along with related user, project, and department details
                var travelRequest = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .Include(tr => tr.Project)
                    .Include(tr => tr.Department)
                    .FirstOrDefaultAsync(tr => tr.TravelRequestId == travelRequestId);

                // Check if travel request exists
                if (travelRequest == null)
                {
                    return NotFound("Travel request not found.");
                }

                // Check if the request is booked (only booked tickets should be downloadable)
                if (travelRequest.Status != "Booked")
                {
                    return BadRequest("Ticket is not yet booked. Only booked tickets can be downloaded.");
                }

                // Generate the PDF using the shared method
                return await GeneratePdfDocument(travelRequest);
            }
            catch (Exception ex)
            {
                // Log the error message if logging is set up
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Generates a PDF document for a travel request
        /// </summary>
        private async Task<IActionResult> GeneratePdfDocument(TravelRequest travelRequest)
        {
            try
            {
                // Create PDF filename
                var pdfFileName = $"TravelRequest_{travelRequest.TravelRequestId}.pdf";
                using (var stream = new MemoryStream())
                {
                    // Create PDF document with page size and margins
                    using (var document = new Document(PageSize.A4, 36, 36, 36, 36))
                    {
                        // Create PdfWriter and attach it to the document
                        using (var writer = PdfWriter.GetInstance(document, stream))
                        {
                            writer.CloseStream = false; // Prevent stream closure by PdfWriter
                            document.Open();

                            // Fonts used in the document
                            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
                            var subTitleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
                            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.DARK_GRAY);

                            // Define colors for headers and rows
                            BaseColor headerColor = new BaseColor(63, 81, 181); // Dark blue for headers
                            BaseColor lightGrayColor = new BaseColor(240, 240, 240); // Light gray for table cells

                            // Add title
                            var title = new Paragraph("Travel Request Confirmation", titleFont)
                            {
                                Alignment = Element.ALIGN_CENTER,
                                SpacingAfter = 20f
                            };
                            document.Add(title);

                            // Add a horizontal line separator
                            document.Add(new Chunk(new LineSeparator(1.0F, 100.0F, BaseColor.LIGHT_GRAY, Element.ALIGN_CENTER, -1)));

                            // Create a table for travel details
                            var travelDetailsTable = new PdfPTable(2) { WidthPercentage = 100 };
                            travelDetailsTable.SetWidths(new float[] { 1, 3 }); // Adjust column widths

                            // Helper method to create table cells with optional background color
                            PdfPCell CreateCell(string text, Font font, int alignment = Element.ALIGN_LEFT, BaseColor bgColor = null)
                            {
                                var cell = new PdfPCell(new Phrase(text, font))
                                {
                                    Border = PdfPCell.BOX,
                                    HorizontalAlignment = alignment,
                                    Padding = 8,
                                    BackgroundColor = bgColor ?? BaseColor.WHITE // Default to white background
                                };
                                return cell;
                            }

                            // Add travel details section with alternating row colors
                            travelDetailsTable.AddCell(CreateCell("From Date:", subTitleFont, Element.ALIGN_LEFT, lightGrayColor));
                            travelDetailsTable.AddCell(CreateCell(travelRequest.FromDate.ToString("yyyy-MM-dd"), normalFont));

                            travelDetailsTable.AddCell(CreateCell("To Date:", subTitleFont, Element.ALIGN_LEFT, lightGrayColor));
                            travelDetailsTable.AddCell(CreateCell(travelRequest.ToDate.ToString("yyyy-MM-dd"), normalFont));

                            travelDetailsTable.AddCell(CreateCell("From Location:", subTitleFont, Element.ALIGN_LEFT, lightGrayColor));
                            travelDetailsTable.AddCell(CreateCell(travelRequest.FromLocation, normalFont));

                            travelDetailsTable.AddCell(CreateCell("To Location:", subTitleFont, Element.ALIGN_LEFT, lightGrayColor));
                            travelDetailsTable.AddCell(CreateCell(travelRequest.ToLocation, normalFont));

                            // Add the travel details table to the document
                            document.Add(travelDetailsTable);

                            // Space before adding the user details
                            document.Add(new Paragraph("\n"));

                            // Add user info with a styled header
                            var userHeading = new Paragraph("Traveler Information", subTitleFont)
                            {
                                SpacingBefore = 10f,
                                SpacingAfter = 5f,
                                Alignment = Element.ALIGN_LEFT
                            };
                            document.Add(userHeading);

                            // Create a table for user details
                            var userInfoTable = new PdfPTable(2) { WidthPercentage = 100 };
                            userInfoTable.SetWidths(new float[] { 1, 3 });

                            // Add user information with alternating colors
                            userInfoTable.AddCell(CreateCell("Name:", subTitleFont, Element.ALIGN_LEFT, lightGrayColor));
                            userInfoTable.AddCell(CreateCell($"{travelRequest.UserName.FirstName} {travelRequest.UserName.LastName}", normalFont));

                            userInfoTable.AddCell(CreateCell("Email:", subTitleFont, Element.ALIGN_LEFT, lightGrayColor));
                            userInfoTable.AddCell(CreateCell(travelRequest.UserName.Email, normalFont));

                            userInfoTable.AddCell(CreateCell("Project:", subTitleFont, Element.ALIGN_LEFT, lightGrayColor));
                            userInfoTable.AddCell(CreateCell(travelRequest.Project.ProjectName, normalFont));

                            userInfoTable.AddCell(CreateCell("Department:", subTitleFont, Element.ALIGN_LEFT, lightGrayColor));
                            userInfoTable.AddCell(CreateCell(travelRequest.Department.DepartmentName, normalFont));

                            // Add user info table to the document
                            document.Add(userInfoTable);

                            // Close the document
                            document.Close();
                        }
                    }

                    // Reset stream position to the start
                    stream.Position = 0;

                    // Return the generated PDF as a downloadable file
                    return File(stream.ToArray(), "application/pdf", pdfFileName);
                }
            }
            catch (Exception ex)
            {
                // Log the error message if logging is set up
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        
                /// <summary>
                /// Returns a travel request to the employee for corrections.
                /// </summary>
                [HttpPost("{travelRequestId}/return-to-employee")]
        public async Task<IActionResult> ReturnToEmployee(int travelRequestId, [FromBody] BookingDetails bookingDetails)
        {
            try
            {
                var travelRequest = await _context.TravelRequests
                    .FirstOrDefaultAsync(tr => tr.TravelRequestId == travelRequestId);

                if (travelRequest == null)
                {
                    return NotFound("Travel request not found.");
                }

                travelRequest.Status = "Returned to Employee";
                travelRequest.Comments = bookingDetails.Comments;

                _context.TravelRequests.Update(travelRequest);
                await _context.SaveChangesAsync();

                return Ok("Request returned to employee successfully.");
            }
            catch (Exception ex)
            {
                // Log exception here if you have a logging mechanism
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Closes a travel request as completed.
        /// </summary>
        [HttpPost("{travelRequestId}/close")]
        public async Task<IActionResult> CloseRequest(int travelRequestId, [FromBody] BookingDetails bookingDetails)
        {
            try
            {
                var travelRequest = await _context.TravelRequests
                    .FirstOrDefaultAsync(tr => tr.TravelRequestId == travelRequestId);

                if (travelRequest == null)
                {
                    return NotFound("Travel request not found.");
                }

                travelRequest.Status = "Completed";
                // Optionally, you can update comments or other fields here

                _context.TravelRequests.Update(travelRequest);
                await _context.SaveChangesAsync();

                return Ok("Request closed successfully.");
            }
            catch (Exception ex)
            {
                // Log exception here if you have a logging mechanism
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns a travel request to the manager for review.
        /// </summary>
        [HttpPost("{travelRequestId}/return-to-manager")]
        public async Task<IActionResult> ReturnToManager(int travelRequestId, [FromBody] BookingDetails bookingDetails)
        {
            try
            {
                var travelRequest = await _context.TravelRequests
                    .FirstOrDefaultAsync(tr => tr.TravelRequestId == travelRequestId);

                if (travelRequest == null)
                {
                    return NotFound("Travel request not found.");
                }

                travelRequest.Status = "Returned to Manager";
                travelRequest.Comments = bookingDetails.Comments;

                _context.TravelRequests.Update(travelRequest);
                await _context.SaveChangesAsync();

                return Ok("Request returned to manager successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets travel admin statistics and analytics.
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var totalRequests = await _context.TravelRequests.CountAsync();
                var pendingRequests = await _context.TravelRequests.CountAsync(tr => tr.Status == "Pending");
                var approvedRequests = await _context.TravelRequests.CountAsync(tr => tr.Status == "Approved");
                var bookedRequests = await _context.TravelRequests.CountAsync(tr => tr.Status == "Booked");
                var completedRequests = await _context.TravelRequests.CountAsync(tr => tr.Status == "Completed");
                var returnedRequests = await _context.TravelRequests.CountAsync(tr => tr.Status.Contains("Returned"));

                // Get requests by department
                var requestsByDepartment = await _context.TravelRequests
                    .Include(tr => tr.Department)
                    .GroupBy(tr => tr.Department.DepartmentName)
                    .Select(g => new
                    {
                        Department = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                // Get requests by status for the last 30 days
                var thirtyDaysAgo = DateTime.Now.AddDays(-30);
                var recentRequests = await _context.TravelRequests
                    .Where(tr => tr.CreatedOn >= thirtyDaysAgo)
                    .GroupBy(tr => tr.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var statistics = new
                {
                    TotalRequests = totalRequests,
                    PendingRequests = pendingRequests,
                    ApprovedRequests = approvedRequests,
                    BookedRequests = bookedRequests,
                    CompletedRequests = completedRequests,
                    ReturnedRequests = returnedRequests,
                    RequestsByDepartment = requestsByDepartment,
                    RecentRequestsByStatus = recentRequests
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets travel requests by status for filtering.
        /// </summary>
        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetRequestsByStatus(string status)
        {
            try
            {
                var travelRequests = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .Include(tr => tr.Project)
                    .Include(tr => tr.Department)
                    .Where(tr => tr.Status == status)
                    .Select(tr => new
                    {
                        tr.TravelRequestId,
                        tr.Status,
                        tr.Comments,
                        tr.FromDate,
                        tr.ToDate,
                        tr.ReasonForTravel,
                        tr.FromLocation,
                        tr.ToLocation,
                        tr.TicketUrl,
                        User = new
                        {
                            tr.UserName.UserId,
                            tr.UserName.FirstName,
                            tr.UserName.LastName,
                            tr.UserName.Email
                        },
                        Project = new
                        {
                            tr.Project.ProjectId,
                            tr.Project.ProjectName
                        },
                        Department = new
                        {
                            tr.Department.DepartmentId,
                            tr.Department.DepartmentName
                        }
                    })
                    .ToListAsync();

                return Ok(travelRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets travel requests by department for filtering.
        /// </summary>
        [HttpGet("by-department/{departmentId}")]
        public async Task<IActionResult> GetRequestsByDepartment(int departmentId)
        {
            try
            {
                var travelRequests = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .Include(tr => tr.Project)
                    .Include(tr => tr.Department)
                    .Where(tr => tr.Department.DepartmentId == departmentId)
                    .Select(tr => new
                    {
                        tr.TravelRequestId,
                        tr.Status,
                        tr.Comments,
                        tr.FromDate,
                        tr.ToDate,
                        tr.ReasonForTravel,
                        tr.FromLocation,
                        tr.ToLocation,
                        tr.TicketUrl,
                        User = new
                        {
                            tr.UserName.UserId,
                            tr.UserName.FirstName,
                            tr.UserName.LastName,
                            tr.UserName.Email
                        },
                        Project = new
                        {
                            tr.Project.ProjectId,
                            tr.Project.ProjectName
                        },
                        Department = new
                        {
                            tr.Department.DepartmentId,
                            tr.Department.DepartmentName
                        }
                    })
                    .ToListAsync();

                return Ok(travelRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets travel requests by date range for filtering.
        /// </summary>
        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetRequestsByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var travelRequests = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .Include(tr => tr.Project)
                    .Include(tr => tr.Department)
                    .Where(tr => tr.FromDate >= startDate && tr.FromDate <= endDate)
                    .Select(tr => new
                    {
                        tr.TravelRequestId,
                        tr.Status,
                        tr.Comments,
                        tr.FromDate,
                        tr.ToDate,
                        tr.ReasonForTravel,
                        tr.FromLocation,
                        tr.ToLocation,
                        tr.TicketUrl,
                        User = new
                        {
                            tr.UserName.UserId,
                            tr.UserName.FirstName,
                            tr.UserName.LastName,
                            tr.UserName.Email
                        },
                        Project = new
                        {
                            tr.Project.ProjectId,
                            tr.Project.ProjectName
                        },
                        Department = new
                        {
                            tr.Department.DepartmentId,
                            tr.Department.DepartmentName
                        }
                    })
                    .ToListAsync();

                return Ok(travelRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets a single travel request by ID with full details.
        /// </summary>
        [HttpGet("{travelRequestId}")]
        public async Task<IActionResult> GetTravelRequestById(int travelRequestId)
        {
            try
            {
                var travelRequest = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .Include(tr => tr.Project)
                    .Include(tr => tr.Department)
                    .Where(tr => tr.TravelRequestId == travelRequestId)
                    .Select(tr => new
                    {
                        tr.TravelRequestId,
                        tr.Status,
                        tr.Comments,
                        tr.FromDate,
                        tr.ToDate,
                        tr.ReasonForTravel,
                        tr.FromLocation,
                        tr.ToLocation,
                        tr.TicketUrl,
                        tr.CreatedOn,
                        User = new
                        {
                            tr.UserName.UserId,
                            tr.UserName.FirstName,
                            tr.UserName.LastName,
                            tr.UserName.Email
                        },
                        Project = new
                        {
                            tr.Project.ProjectId,
                            tr.Project.ProjectName
                        },
                        Department = new
                        {
                            tr.Department.DepartmentId,
                            tr.Department.DepartmentName
                        }
                    })
                    .FirstOrDefaultAsync();

                if (travelRequest == null)
                {
                    return NotFound("Travel request not found.");
                }

                return Ok(travelRequest);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
