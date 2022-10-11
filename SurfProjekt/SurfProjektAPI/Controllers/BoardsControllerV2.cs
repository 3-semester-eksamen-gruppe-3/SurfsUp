using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SurfProjekt.Models;
using SurfProjektAPI.Data;


namespace SurfProjektAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class BoardsControllerV2 : ControllerBase
    {
        private readonly SurfProjektContext _context;
        //private readonly UserManager<IdentityUser> userManager;

        public BoardsControllerV2(SurfProjektContext context)
        {
            _context = context;
            //userManager = UserManager;
        }

        // GET: api/Boards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Boards>>> GetBoards()
        {

            return await _context.Boards.Include(b => b.leases).AsNoTracking().Where(b => b.IsPremium == false).ToListAsync();
        }

        // GET: api/Boards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Boards>> GetBoards(int id)
        {
            var boards = await _context.Boards.FindAsync(id);

            if (boards == null)
            {
                return NotFound();
            }

            if (boards.IsPremium)
            {
                return Unauthorized("Du har ikke tilladelse til at se dette surfboard. Log ind for at se den fulde oversigt.");
            }

            return boards;
        }

        [HttpPost("Rent/{id}")]
        public async Task<ActionResult<Boards>> Rent(int id, [FromBody]Lease lease)
        {
            if (_context.Boards == null)
            {
                return Problem("Entity set 'SurfProjektContext.Boards'  is null.");
            }
            var boards = await _context.Boards.FindAsync(id);
            //var user = await userManager.GetUserAsync(User);
            if (boards == null)
            {
                return NotFound();
            }

            if (boards.IsPremium)
            {
                return Unauthorized("Du har ikke tilladelse til at leje dette surfboard. Log ind for at få adgang til alle boards.");
            }


            boards.leases = new List<Lease>();
            lease.Date = DateTime.Now;
            lease.EndTime = lease.Date.AddHours(lease.TimeFrame);
            lease.BoardID = id;
            boards.leases.Add(lease);

            //boards.IsRented = true;

            await _context.SaveChangesAsync();
            return Ok();
        }


        // PUT: api/Boards/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutBoards(int id, Boards boards)
        //{
        //    if (id != boards.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(boards).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!BoardsExists(id))
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

        //// POST: api/Boards
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Boards>> PostBoards(Boards boards)
        //{
        //    _context.Boards.Add(boards);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetBoards", new { id = boards.Id }, boards);
        //}

        //// DELETE: api/Boards/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteBoards(int id)
        //{
        //    var boards = await _context.Boards.FindAsync(id);
        //    if (boards == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Boards.Remove(boards);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool BoardsExists(int id)
        {
            return _context.Boards.Any(e => e.Id == id);
        }
    }
}
