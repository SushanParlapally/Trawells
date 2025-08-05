
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
   
    using System.Collections.Generic;
    using System.Linq;
    using TravelDesk.Data;
    using TravelDesk.Models;

namespace TravelDesk.Controllers
{
    
        [ApiController]
        [Route("api/[controller]")]
        public class RoleController : ControllerBase
        {
            private readonly TravelDeskContext _context;

            public RoleController(TravelDeskContext context)
            {
                _context = context;
            }

            // GET: api/role
            [HttpGet]
            public ActionResult<IEnumerable<Role>> GetRoles()
            {
                var roles = _context.Roles.Where(r => r.RoleName != "Admin").ToList();
                return Ok(roles);
            }

            // GET: api/role/{id}
            [HttpGet("{id}")]
            public ActionResult<Role> GetRole(int id)
            {
                var role = _context.Roles.Find(id);
                if (role == null)
                {
                    return NotFound();
                }
                return Ok(role);
            }

            
          
        }
    
}
