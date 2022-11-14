using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurfProjektBlazor.Server.Models;

namespace SurfProjektBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private SignInManager<ApplicationUser> _signInManager;
        public UserController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<bool> GetUserAuthentication()
        {
            return _signInManager.IsSignedIn(User);
        }
    }
}
