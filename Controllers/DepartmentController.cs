using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using TravelDesk.Data;
using TravelDesk.Models;
using TravelDesk.DTO;
using System;

namespace TravekDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly TravelDeskContext _context;

        public DepartmentController(TravelDeskContext context)
        {
            _context = context;
        }

        // GET: api/department
        [HttpGet]
        public ActionResult<IEnumerable<Department>> GetDepartments()
        {
            var departments = _context.Departments.ToList();
            return Ok(departments);
        }

        /// <summary>
        /// Gets department statistics.
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetDepartmentStatistics()
        {
            try
            {
                var totalDepartments = await _context.Departments.CountAsync();
                var activeDepartments = await _context.Departments.CountAsync(d => d.IsActive);

                // Get users by department
                var usersByDepartment = await _context.Users
                    .Include(u => u.Department)
                    .Where(u => u.IsActive)
                    .GroupBy(u => u.Department != null ? u.Department.DepartmentName : "Unknown")
                    .Select(g => new
                    {
                        Department = g.Key,
                        UserCount = g.Count()
                    })
                    .ToListAsync();

                // Get travel requests by department
                var requestsByDepartment = await _context.TravelRequests
                    .Include(tr => tr.Department)
                    .GroupBy(tr => tr.Department != null ? tr.Department.DepartmentName : "Unknown")
                    .Select(g => new
                    {
                        Department = g.Key,
                        RequestCount = g.Count()
                    })
                    .ToListAsync();

                var statistics = new
                {
                    TotalDepartments = totalDepartments,
                    ActiveDepartments = activeDepartments,
                    UsersByDepartment = usersByDepartment,
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
        /// Gets department by ID with user count.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            try
            {
                var department = await _context.Departments
                    .Where(d => d.DepartmentId == id)
                    .Select(d => new
                    {
                        d.DepartmentId,
                        d.DepartmentName,
                        d.IsActive,
                        UserCount = d.Users.Count(u => u.IsActive),
                        RequestCount = d.TravelRequests.Count
                    })
                    .FirstOrDefaultAsync();

                if (department == null)
                {
                    return NotFound("Department not found.");
                }

                return Ok(department);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets users in a specific department.
        /// </summary>
        [HttpGet("{id}/users")]
        public async Task<IActionResult> GetDepartmentUsers(int id)
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Manager)
                    .Where(u => u.DepartmentId == id && u.IsActive)
                    .Select(u => new
                    {
                        u.UserId,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.Address,
                        Role = u.Role != null ? new
                        {
                            u.Role.RoleId,
                            u.Role.RoleName
                        } : null,
                        Manager = u.Manager != null ? new
                        {
                            u.Manager.UserId,
                            u.Manager.FirstName,
                            u.Manager.LastName
                        } : null
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets travel requests for a specific department.
        /// </summary>
        [HttpGet("{id}/travel-requests")]
        public async Task<IActionResult> GetDepartmentTravelRequests(int id)
        {
            try
            {
                var requests = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .Include(tr => tr.Project)
                    .Where(tr => tr.Department.DepartmentId == id)
                    .Select(tr => new
                    {
                        tr.TravelRequestId,
                        tr.Status,
                        tr.FromDate,
                        tr.ToDate,
                        tr.FromLocation,
                        tr.ToLocation,
                        tr.ReasonForTravel,
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
                        }
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
       
       
    }
}
