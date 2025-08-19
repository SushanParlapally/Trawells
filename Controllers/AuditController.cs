using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TravelDesk.Data;
using TravelDesk.Models;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace TravelDesk.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly TravelDeskContext _context;

        public AuditController(TravelDeskContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuditLog>>> GetAuditLogs(
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.AuditLogs.AsQueryable();

            var totalCount = await query.CountAsync();
            var auditLogs = await query
                .OrderByDescending(al => al.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            Response.Headers.Append("X-Total-Count", totalCount.ToString());
            Response.Headers.Append("X-Page-Number", pageNumber.ToString());
            Response.Headers.Append("X-Page-Size", pageSize.ToString());

            return Ok(auditLogs);
        }
    }
}