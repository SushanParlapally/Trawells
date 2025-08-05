using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelDesk.Data;
using TravelDesk.Models;
using TravelDesk.DTO;
using System;

namespace TravelDesk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly TravelDeskContext _context;

        public ProjectController(TravelDeskContext context)
        {
            _context = context;
        }

        // GET: api/Project
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }

        // GET: api/Project/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        /// <summary>
        /// Gets project statistics.
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetProjectStatistics()
        {
            try
            {
                var totalProjects = await _context.Projects.CountAsync();
                var activeProjects = await _context.Projects.CountAsync(p => p.IsActive);

                // Get travel requests by project
                var requestsByProject = await _context.TravelRequests
                    .Include(tr => tr.Project)
                    .GroupBy(tr => tr.Project.ProjectName)
                    .Select(g => new
                    {
                        Project = g.Key,
                        RequestCount = g.Count()
                    })
                    .ToListAsync();

                // Get projects with most travel requests
                var topProjects = await _context.TravelRequests
                    .Include(tr => tr.Project)
                    .GroupBy(tr => new { tr.Project.ProjectId, tr.Project.ProjectName })
                    .Select(g => new
                    {
                        ProjectId = g.Key.ProjectId,
                        ProjectName = g.Key.ProjectName,
                        RequestCount = g.Count()
                    })
                    .OrderByDescending(p => p.RequestCount)
                    .Take(5)
                    .ToListAsync();

                var statistics = new
                {
                    TotalProjects = totalProjects,
                    ActiveProjects = activeProjects,
                    RequestsByProject = requestsByProject,
                    TopProjects = topProjects
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets project by ID with detailed information.
        /// </summary>
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetProjectDetails(int id)
        {
            try
            {
                var project = await _context.Projects
                    .Where(p => p.ProjectId == id)
                    .Select(p => new
                    {
                        p.ProjectId,
                        p.ProjectName,
                        p.IsActive,
                        RequestCount = p.TravelRequests.Count,
                        RecentRequests = p.TravelRequests
                            .OrderByDescending(tr => tr.CreatedOn)
                            .Take(5)
                            .Select(tr => new
                            {
                                tr.TravelRequestId,
                                tr.Status,
                                tr.CreatedOn,
                                User = new
                                {
                                    tr.UserName.FirstName,
                                    tr.UserName.LastName
                                }
                            })
                            .ToList()
                    })
                    .FirstOrDefaultAsync();

                if (project == null)
                {
                    return NotFound("Project not found.");
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets travel requests for a specific project.
        /// </summary>
        [HttpGet("{id}/travel-requests")]
        public async Task<IActionResult> GetProjectTravelRequests(int id)
        {
            try
            {
                var requests = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .Include(tr => tr.Department)
                    .Where(tr => tr.Project.ProjectId == id)
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
                        Department = new
                        {
                            tr.Department.DepartmentId,
                            tr.Department.DepartmentName
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

        /// <summary>
        /// Gets projects by status (active/inactive).
        /// </summary>
        [HttpGet("by-status/{isActive}")]
        public async Task<IActionResult> GetProjectsByStatus(bool isActive)
        {
            try
            {
                var projects = await _context.Projects
                    .Where(p => p.IsActive == isActive)
                    .Select(p => new
                    {
                        p.ProjectId,
                        p.ProjectName,
                        p.IsActive,
                        RequestCount = p.TravelRequests.Count
                    })
                    .ToListAsync();

                return Ok(projects);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
       
       
    }
}
