using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TravelDesk.Data;
using TravelDesk.DTO;
using TravelDesk.Models;
using System;

namespace TravelDesk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagerController : ControllerBase
    {
        private readonly TravelDeskContext _context;

        public ManagerController(TravelDeskContext context)
        {
            _context = context;
        }

        // GET: api/Manager/{managerId}/Requests
        [HttpGet("{managerId}/Requests")]
        public async Task<ActionResult<IEnumerable<TravelRequestDto>>> GetPendingRequests(int managerId)
        {
            var pendingRequests = await _context.TravelRequests
                .Where(tr => tr.UserName != null && tr.UserName.ManagerId == managerId)
                .Include(tr => tr.Project)
                .Include(tr => tr.UserName)
                .Select(tr => new TravelRequestDto
                {
                    TravelRequestId = tr.TravelRequestId,
                    Project = tr.Project != null ? new ProjectDto
                    {
                        ProjectId = tr.Project.ProjectId,
                        ProjectName = tr.Project.ProjectName
                    } : new ProjectDto(),
                    User = tr.UserName != null ? new UserDto
                    {
                        UserId = tr.UserName.UserId,
                        FirstName = tr.UserName.FirstName ?? string.Empty,
                        LastName = tr.UserName.LastName ?? string.Empty
                       
                    } : new UserDto(),
                    ReasonForTravel = tr.ReasonForTravel,
                    FromDate = tr.FromDate,
                    ToDate = tr.ToDate,
                    FromLocation = tr.FromLocation,
                    ToLocation = tr.ToLocation,
                    Status = tr.Status,
                    Comments = tr.Comments
                })
                .ToListAsync();

            if (!pendingRequests.Any())
            {
                return NotFound("No pending requests found for this manager.");
            }

            return Ok(pendingRequests);
        }
        [HttpPut("ApproveRequest/{requestId}")]
        public async Task<IActionResult> ApproveRequest(int requestId, [FromBody] CommentDto commentDto)
        {
            var travelRequest = await _context.TravelRequests.FindAsync(requestId);
            if (travelRequest == null)
            {
                return NotFound();
            }

            travelRequest.Status = "Approved";
            travelRequest.Comments = commentDto.Comments; // Access comments from the DTO
            _context.Entry(travelRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TravelRequestExists(requestId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPut("RejectRequest/{requestId}")]
        public async Task<IActionResult> RejectRequest(int requestId, [FromBody] CommentDto commentDto)
        {
            var travelRequest = await _context.TravelRequests.FindAsync(requestId);
            if (travelRequest == null)
            {
                return NotFound();
            }

            travelRequest.Status = "Rejected";
            travelRequest.Comments = commentDto.Comments; // Access comments from the DTO
            _context.Entry(travelRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TravelRequestExists(requestId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        private bool TravelRequestExists(int id)
        {
            return _context.TravelRequests.Any(e => e.TravelRequestId == id);
        }

        /// <summary>
        /// Gets manager dashboard statistics.
        /// </summary>
        [HttpGet("{managerId}/statistics")]
        public async Task<IActionResult> GetManagerStatistics(int managerId)
        {
            try
            {
                var totalRequests = await _context.TravelRequests
                    .CountAsync(tr => tr.UserName != null && tr.UserName.ManagerId == managerId);

                var pendingRequests = await _context.TravelRequests
                    .CountAsync(tr => tr.UserName != null && tr.UserName.ManagerId == managerId && tr.Status == "Pending");

                var approvedRequests = await _context.TravelRequests
                    .CountAsync(tr => tr.UserName != null && tr.UserName.ManagerId == managerId && tr.Status == "Approved");

                var rejectedRequests = await _context.TravelRequests
                    .CountAsync(tr => tr.UserName != null && tr.UserName.ManagerId == managerId && tr.Status == "Rejected");

                var completedRequests = await _context.TravelRequests
                    .CountAsync(tr => tr.UserName != null && tr.UserName.ManagerId == managerId && tr.Status == "Completed");

                var teamMembersCount = await _context.Users
                    .CountAsync(u => u.ManagerId == managerId && u.IsActive);

                // Get requests by status for the last 30 days
                var thirtyDaysAgo = DateTime.Now.AddDays(-30);
                var recentRequests = await _context.TravelRequests
                    .Where(tr => tr.UserName != null && tr.UserName.ManagerId == managerId && tr.CreatedOn >= thirtyDaysAgo)
                    .GroupBy(tr => tr.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                // Get requests by department for this manager's team
                var requestsByDepartment = await _context.TravelRequests
                    .Include(tr => tr.Department)
                    .Where(tr => tr.UserName != null && tr.UserName.ManagerId == managerId)
                    .GroupBy(tr => tr.Department != null ? tr.Department.DepartmentName : "Unknown")
                    .Select(g => new
                    {
                        Department = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                var statistics = new
                {
                    TotalRequests = totalRequests,
                    PendingRequests = pendingRequests,
                    ApprovedRequests = approvedRequests,
                    RejectedRequests = rejectedRequests,
                    CompletedRequests = completedRequests,
                    TeamMembersCount = teamMembersCount,
                    RecentRequestsByStatus = recentRequests,
                    RequestsByDepartment = requestsByDepartment
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets all requests for a manager (not just pending).
        /// </summary>
        [HttpGet("{managerId}/all-requests")]
        public async Task<ActionResult<IEnumerable<TravelRequestDto>>> GetAllManagerRequests(int managerId)
        {
            try
            {
                var allRequests = await _context.TravelRequests
                    .Where(tr => tr.UserName != null && tr.UserName.ManagerId == managerId)
                    .Include(tr => tr.Project)
                    .Include(tr => tr.UserName)
                    .Include(tr => tr.Department)
                    .Select(tr => new TravelRequestDto
                    {
                        TravelRequestId = tr.TravelRequestId,
                        Project = tr.Project != null ? new ProjectDto
                        {
                            ProjectId = tr.Project.ProjectId,
                            ProjectName = tr.Project.ProjectName
                        } : new ProjectDto(),
                        User = tr.UserName != null ? new UserDto
                        {
                            UserId = tr.UserName.UserId,
                            FirstName = tr.UserName.FirstName ?? string.Empty,
                            LastName = tr.UserName.LastName ?? string.Empty
                        } : new UserDto(),
                        ReasonForTravel = tr.ReasonForTravel,
                        FromDate = tr.FromDate,
                        ToDate = tr.ToDate,
                        FromLocation = tr.FromLocation,
                        ToLocation = tr.ToLocation,
                        Status = tr.Status,
                        Comments = tr.Comments,
                        CreatedOn = tr.CreatedOn
                    })
                    .OrderByDescending(tr => tr.CreatedOn)
                    .ToListAsync();

                return Ok(allRequests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets requests by status for a manager.
        /// </summary>
        [HttpGet("{managerId}/requests-by-status/{status}")]
        public async Task<ActionResult<IEnumerable<TravelRequestDto>>> GetRequestsByStatus(int managerId, string status)
        {
            try
            {
                var requests = await _context.TravelRequests
                    .Where(tr => tr.UserName != null && tr.UserName.ManagerId == managerId && tr.Status == status)
                    .Include(tr => tr.Project)
                    .Include(tr => tr.UserName)
                    .Include(tr => tr.Department)
                    .Select(tr => new TravelRequestDto
                    {
                        TravelRequestId = tr.TravelRequestId,
                        Project = tr.Project != null ? new ProjectDto
                        {
                            ProjectId = tr.Project.ProjectId,
                            ProjectName = tr.Project.ProjectName
                        } : new ProjectDto(),
                        User = tr.UserName != null ? new UserDto
                        {
                            UserId = tr.UserName.UserId,
                            FirstName = tr.UserName.FirstName ?? string.Empty,
                            LastName = tr.UserName.LastName ?? string.Empty
                        } : new UserDto(),
                        ReasonForTravel = tr.ReasonForTravel,
                        FromDate = tr.FromDate,
                        ToDate = tr.ToDate,
                        FromLocation = tr.FromLocation,
                        ToLocation = tr.ToLocation,
                        Status = tr.Status,
                        Comments = tr.Comments,
                        CreatedOn = tr.CreatedOn
                    })
                    .OrderByDescending(tr => tr.CreatedOn)
                    .ToListAsync();

                return Ok(requests);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets team members for a manager.
        /// </summary>
        [HttpGet("{managerId}/team-members")]
        public async Task<IActionResult> GetTeamMembers(int managerId)
        {
            try
            {
                var teamMembers = await _context.Users
                    .Include(u => u.Department)
                    .Include(u => u.Role)
                    .Where(u => u.ManagerId == managerId && u.IsActive)
                    .Select(u => new
                    {
                        u.UserId,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.Address,
                        Department = u.Department != null ? new
                        {
                            u.Department.DepartmentId,
                            u.Department.DepartmentName
                        } : null,
                        Role = u.Role != null ? new
                        {
                            u.Role.RoleId,
                            u.Role.RoleName
                        } : null
                    })
                    .ToListAsync();

                return Ok(teamMembers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
