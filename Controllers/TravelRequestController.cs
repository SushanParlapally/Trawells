using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelDesk.Data;
using TravelDesk.DTO;
using TravelDesk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelDesk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelRequestController : ControllerBase
    {
        private readonly TravelDeskContext _context;

        public TravelRequestController(TravelDeskContext context)
        {
            _context = context;
        }

        // GET: api/TravelRequest
        [HttpGet]
        public async Task<ActionResult<DashboardDto>> GetTravelRequests()
        {
            var travelRequests = await _context.TravelRequests
                .Include(tr => tr.UserName)
                .Include(tr => tr.Project)
                .Include(tr => tr.UserName!.Department)
                .Select(tr => new TravelRequestDto
                {
                    TravelRequestId = tr.TravelRequestId,
                    User = tr.UserName != null ? new UserDto
                    {
                        UserId = tr.UserName.UserId,
                        FirstName = tr.UserName.FirstName ?? string.Empty,
                        LastName = tr.UserName.LastName ?? string.Empty,
                        Department = tr.UserName.Department != null ? new DepartmentDto
                        {
                            DepartmentId = tr.UserName.Department.DepartmentId,
                            DepartmentName = tr.UserName.Department.DepartmentName
                        } : new DepartmentDto()
                    } : new UserDto(),
                    Project = tr.Project != null ? new ProjectDto
                    {
                        ProjectId = tr.Project.ProjectId,
                        ProjectName = tr.Project.ProjectName
                    } : new ProjectDto(),
                    ReasonForTravel = tr.ReasonForTravel,
                    FromDate = tr.FromDate,
                    ToDate = tr.ToDate,
                    FromLocation = tr.FromLocation,
                    ToLocation = tr.ToLocation,
                    Status = tr.Status,
                    Comments = tr.Comments
                })
                .ToListAsync();

            var dashboard = new DashboardDto
            {
                Requests = travelRequests
            };

            return Ok(dashboard);
        }
        [HttpPost]
        public async Task<IActionResult> PostTravelRequest(TravelRequest travelRequest)
        {
            // Ensure status and comments are not included
            travelRequest.Status = "Pending"; // Default status
            travelRequest.Comments = null; // Default comments

            _context.TravelRequests.Add(travelRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTravelRequests), new { id = travelRequest.TravelRequestId }, travelRequest);
        }


        //// PUT: api/TravelRequest/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateTravelRequest(int id, TravelRequestDto travelRequestDto)
        //{
        //    var travelRequest = await _context.TravelRequests.FindAsync(id);

        //    if (travelRequest == null)
        //    {
        //        return NotFound();
        //    }

        //    travelRequest.Status = travelRequestDto.Status;
        //    travelRequest.Comments = travelRequestDto.Comments;
        //    travelRequest.ModifiedOn = DateTime.Now;

        //    _context.Entry(travelRequest).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TravelRequestExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TravelRequestDto>>> GetTravelRequestsByUserId(int userId)
        {
            var travelRequests = await _context.TravelRequests
                .Where(tr => tr.UserId == userId)
                .Include(tr => tr.UserName)
                .Include(tr => tr.Project)
                .Include(tr => tr.UserName!.Department)
                .Select(tr => new TravelRequestDto
                {
                    TravelRequestId = tr.TravelRequestId,
                    User = tr.UserName != null ? new UserDto
                    {
                        UserId = tr.UserName.UserId,
                        FirstName = tr.UserName.FirstName ?? string.Empty,
                        LastName = tr.UserName.LastName ?? string.Empty,
                        Department = tr.UserName.Department != null ? new DepartmentDto
                        {
                            DepartmentId = tr.UserName.Department.DepartmentId,
                            DepartmentName = tr.UserName.Department.DepartmentName
                        } : new DepartmentDto()
                    } : new UserDto(),
                    Project = tr.Project != null ? new ProjectDto
                    {
                        ProjectId = tr.Project.ProjectId,
                        ProjectName = tr.Project.ProjectName
                    } : new ProjectDto(),
                    ReasonForTravel = tr.ReasonForTravel,
                    FromDate = tr.FromDate,
                    ToDate = tr.ToDate,
                    FromLocation = tr.FromLocation,
                    ToLocation = tr.ToLocation,
                    Status = tr.Status,
                    Comments = tr.Comments
                })
                .ToListAsync();

            if (travelRequests == null || !travelRequests.Any())
            {
                return NotFound("No travel requests found for this user.");
            }

            return Ok(travelRequests);
        }

        //[HttpGet("{TravelRequestId}")]
        //public async Task<ActionResult<TravelRequestDto>> GetTravelRequest(int TravelRequestId)
        //{
        //    var travelRequest = await _context.TravelRequests
        //        .Include(tr => tr.User)
        //        .ThenInclude(u => u.Department)
        //        .Include(tr => tr.Project)
        //        .Where(tr => tr.TravelRequestId == TravelRequestId)
        //        .Select(tr => new TravelRequestDto
        //        {
        //            TravelRequestId = tr.TravelRequestId,
        //            User = new UserDto
        //            {
        //                UserId = tr.User.UserId,
        //                FirstName = tr.User.FirstName,
        //                LastName = tr.User.LastName,
        //                Department = new DepartmentDto
        //                {
        //                    DepartmentId = tr.User.Department.DepartmentId,
        //                    DepartmentName = tr.User.Department.DepartmentName
        //                }
        //            },
        //            Project = new ProjectDto
        //            {
        //                ProjectId = tr.Project.ProjectId,
        //                ProjectName = tr.Project.ProjectName
        //            },
        //            ReasonForTravel = tr.ReasonForTravel,
        //            FromDate = tr.FromDate,
        //            ToDate = tr.ToDate,
        //            FromLocation = tr.FromLocation,
        //            ToLocation = tr.ToLocation,
        //            Status = tr.Status,
        //            Comments = tr.Comments
        //        })
        //        .FirstOrDefaultAsync();

        //    if (travelRequest == null)
        //    {
        //        return NotFound($"Travel request with ID {TravelRequestId} not found.");
        //    }

        //    return Ok(travelRequest);
        //}

        private bool TravelRequestExists(int id)
        {
            return _context.TravelRequests.Any(e => e.TravelRequestId == id);
        }
    }
}
