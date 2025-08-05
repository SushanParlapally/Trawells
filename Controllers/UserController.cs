using Microsoft.AspNetCore.Authorization;
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

    public class UserController : ControllerBase
    {
        private readonly TravelDeskContext _context;

        private readonly IConfiguration _configuration;



        public UserController(TravelDeskContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

        }

        [HttpGet("users")]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            var users = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Include(u => u.Manager)
                .Where(u=>u.Role.RoleName!="Admin" && u.IsActive==true)
                .ToList();

            return Ok(users);
        }



        [HttpPost("users")]
        public ActionResult<User> AddUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Users.Add(user);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetUsers), new { id = user.UserId }, user);
        }

        [HttpPut("users/{id}")]
        public IActionResult UpdateUser(int id, User updateUser)
        {
            if (id != updateUser.UserId)
            {
                return BadRequest();
            }

            var existingUser = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Department)
                .Include(u => u.Manager)
                .FirstOrDefault(u => u.UserId == id);

            if (existingUser == null)
            {
                return NotFound();
            }

            // Update only specific fields
            existingUser.FirstName = updateUser.FirstName ?? existingUser.FirstName;
            existingUser.LastName = updateUser.LastName ?? existingUser.LastName;
            existingUser.Password = updateUser.Password ?? existingUser.Password;
            existingUser.Address = updateUser.Address ?? existingUser.Address;

            // Save changes
            _context.Entry(existingUser).State = EntityState.Modified;
            _context.SaveChanges();

            return NoContent();
        }



        [HttpDelete("users/{id}")]
        public IActionResult DeleteUser(int id)
        {

            var user = _context.Users.SingleOrDefault(x=>x.UserId==id && x.IsActive==true);
            if (user == null)
            {
                return NotFound();
            }
            user.IsActive= false; 
           
            _context.SaveChanges();
            return NoContent();
        }
        [HttpGet("managers")]
        public IActionResult GetManagers()
        {
            var users = _context.Users
               .Include(u => u.Role)
               .Include(u => u.Department)
               .Include(u => u.Manager)
               .Where(u => u.RoleId == 3)
               .ToList();

            return Ok(users);

        }

        //[HttpGet("current")]
        //// Assuming you want to restrict this endpoint to authorized users only
        //public async Task<ActionResult<User>> GetCurrentUser()
        //{
        //    // Assuming the user ID is available in the Claims of the authenticated user
        //    var userId = User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;

        //    if (userId == null)
        //    {
        //        return Unauthorized();
        //    }

        //    var user = await _context.Users
        //        .Include(u => u.Department)
        //        .Where(u => u.UserId == int.Parse(userId))
        //        .FirstOrDefaultAsync();

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    // Optional: Select specific properties to return
        //    var userResponse = new
        //    {
        //        user.UserId,
        //        user.FirstName,
        //        user.LastName,
        //        DepartmentName = user.Department.DepartmentName
        //    };

        //    return Ok(userResponse);
        //}

        /// <summary>
        /// Gets system-wide statistics for admin dashboard.
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetSystemStatistics()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync(u => u.IsActive);
                var totalManagers = await _context.Users.CountAsync(u => u.RoleId == 3 && u.IsActive);
                var totalEmployees = await _context.Users.CountAsync(u => u.RoleId == 2 && u.IsActive);
                var totalTravelAdmins = await _context.Users.CountAsync(u => u.RoleId == 4 && u.IsActive);

                var totalTravelRequests = await _context.TravelRequests.CountAsync();
                var pendingRequests = await _context.TravelRequests.CountAsync(tr => tr.Status == "Pending");
                var approvedRequests = await _context.TravelRequests.CountAsync(tr => tr.Status == "Approved");
                var completedRequests = await _context.TravelRequests.CountAsync(tr => tr.Status == "Completed");

                var totalDepartments = await _context.Departments.CountAsync();
                var totalProjects = await _context.Projects.CountAsync();

                // Get users by department
                var usersByDepartment = await _context.Users
                    .Include(u => u.Department)
                    .Where(u => u.IsActive)
                    .GroupBy(u => u.Department.DepartmentName)
                    .Select(g => new
                    {
                        Department = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

                // Get recent activity (last 30 days)
                var thirtyDaysAgo = DateTime.Now.AddDays(-30);
                var recentTravelRequests = await _context.TravelRequests
                    .Where(tr => tr.CreatedOn >= thirtyDaysAgo)
                    .CountAsync();

                var statistics = new
                {
                    Users = new
                    {
                        Total = totalUsers,
                        Managers = totalManagers,
                        Employees = totalEmployees,
                        TravelAdmins = totalTravelAdmins,
                        ByDepartment = usersByDepartment
                    },
                    TravelRequests = new
                    {
                        Total = totalTravelRequests,
                        Pending = pendingRequests,
                        Approved = approvedRequests,
                        Completed = completedRequests,
                        Recent = recentTravelRequests
                    },
                    System = new
                    {
                        Departments = totalDepartments,
                        Projects = totalProjects
                    }
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets user activity statistics.
        /// </summary>
        [HttpGet("activity")]
        public async Task<IActionResult> GetUserActivity()
        {
            try
            {
                // Get recent travel requests with user details
                var recentRequests = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .Include(tr => tr.Department)
                    .OrderByDescending(tr => tr.CreatedOn)
                    .Take(10)
                    .Select(tr => new
                    {
                        tr.TravelRequestId,
                        tr.Status,
                        tr.CreatedOn,
                        User = new
                        {
                            tr.UserName.FirstName,
                            tr.UserName.LastName,
                            tr.UserName.Email
                        },
                        Department = tr.Department.DepartmentName
                    })
                    .ToListAsync();

                // Get users with most travel requests
                var activeUsers = await _context.TravelRequests
                    .Include(tr => tr.UserName)
                    .GroupBy(tr => new { tr.UserName.UserId, tr.UserName.FirstName, tr.UserName.LastName })
                    .Select(g => new
                    {
                        UserId = g.Key.UserId,
                        FirstName = g.Key.FirstName,
                        LastName = g.Key.LastName,
                        RequestCount = g.Count()
                    })
                    .OrderByDescending(u => u.RequestCount)
                    .Take(5)
                    .ToListAsync();

                var activity = new
                {
                    RecentRequests = recentRequests,
                    ActiveUsers = activeUsers
                };

                return Ok(activity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets users by role for filtering.
        /// </summary>
        [HttpGet("by-role/{roleId}")]
        public async Task<IActionResult> GetUsersByRole(int roleId)
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Department)
                    .Include(u => u.Manager)
                    .Where(u => u.RoleId == roleId && u.IsActive)
                    .Select(u => new
                    {
                        u.UserId,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.Address,
                        u.IsActive,
                        Role = new
                        {
                            u.Role.RoleId,
                            u.Role.RoleName
                        },
                        Department = new
                        {
                            u.Department.DepartmentId,
                            u.Department.DepartmentName
                        },
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
        /// Gets users by department for filtering.
        /// </summary>
        [HttpGet("by-department/{departmentId}")]
        public async Task<IActionResult> GetUsersByDepartment(int departmentId)
        {
            try
            {
                var users = await _context.Users
                    .Include(u => u.Role)
                    .Include(u => u.Department)
                    .Include(u => u.Manager)
                    .Where(u => u.DepartmentId == departmentId && u.IsActive)
                    .Select(u => new
                    {
                        u.UserId,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.Address,
                        u.IsActive,
                        Role = new
                        {
                            u.Role.RoleId,
                            u.Role.RoleName
                        },
                        Department = new
                        {
                            u.Department.DepartmentId,
                            u.Department.DepartmentName
                        },
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
    }
    }
