using Microsoft.AspNetCore.Mvc;
using SurfProjektBlazor.Server.Data;
using SurfProjektBlazor.Shared;

namespace SurfProjektBlazor.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        //private readonly UserManager<IdentityUser> userManager;

        public BoardsController(ApplicationDbContext context)
        {
            _context = context;
            //userManager = UserManager;
        }

    }
}
