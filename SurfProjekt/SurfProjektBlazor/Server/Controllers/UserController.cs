using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurfProjektBlazor.Server.Data;

namespace SurfProjektBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private SignInManager<IdentityUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _context;
        public UserController(SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public async Task<bool> GetUserAuthentication()
        {
            return _signInManager.IsSignedIn(User);
        }

        [HttpGet ("{role}")] 
        public async Task<bool> AddRole(string role)
        {
            var newRole = new IdentityRole(role);
            var result = await _roleManager.CreateAsync(newRole);
            return result.Succeeded;
        }

        [HttpGet ("roles")]
        public async Task<IEnumerable<IdentityRole>> GetRoles()
        {
            return _context.Roles.AsEnumerable<IdentityRole>();
        }
    }
}
