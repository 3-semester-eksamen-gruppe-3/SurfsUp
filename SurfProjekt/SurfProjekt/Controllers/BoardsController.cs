using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SurfProjekt;
using SurfProjekt.Data;
using SurfProjekt.Models;
using SurfProjekt.Models.ViewModels;
using static System.Net.WebRequestMethods;

namespace SurfProjekt.Controllers
{
    
    //Denne línje angiver at begge roller, admin og user, har adgang til boards-siden.
    //[Authorize(Roles = $"{ConstantsRole.Roles.Admin}, {ConstantsRole.Roles.User}")]
    
    //Denne linje sørger for, at man godt kan se borads-siden uden at oprette en bruger.
    //[AllowAnonymous]
    public class BoardsController : Controller
    {
        private UserManager<IdentityUser> userManager;
        private readonly SurfProjektContext _context;

        public BoardsController(SurfProjektContext context, UserManager<IdentityUser> UserManager)
        {
            _context = context;
            userManager = UserManager;
        }

        // GET: Boards
        //public async Task<IActionResult> Index()
        //{
        //      return _context.Boards != null ? 
        //                  View(await _context.Boards.ToListAsync()) :
        //                  Problem("Entity set 'SurfProjektContext.Boards'  is null.");
        //}

        public async Task<IActionResult> Index(
        string currentFilter,
        string searchString,
        int? pageNumber)
        {
            //var boards = from b in _context.Boards.
            //             Include(b => b.leases).
            //             AsNoTracking().
            //             Where(b => b.IsRented != true);
            string URL = "https://localhost:7244/api/Boards";
            BoardHierarchy boardHierarchy = new();
            HttpClient boardsClient= new HttpClient();
            var boards= await boardsClient.GetFromJsonAsync<IEnumerable<Boards>>(URL);

            //var boards = _context.Boards.Include(b => b.leases).AsNoTracking();

            foreach(var board in boards)
            {
                foreach(var lease in board.leases)
                {
                    if (lease.Date < DateTime.Now && lease.EndTime > DateTime.Now)
                    {
                        board.IsRented = true;
                    }
                    else
                    {
                        board.IsRented = false;
                    }
                    _context.Update(board);

                }
            }
            await _context.SaveChangesAsync();

            boards = from b in boards
                     where !(b.IsRented == true)
                     select b;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            if (!String.IsNullOrEmpty(searchString))
            {
                //Hvorfor er ! bagved name og ikke contains?
                boards = boards.Where(b => b.Name!.Contains(searchString)
                                        || b.Type.Contains(searchString));
            }

            int pageSize = 4;
            boardHierarchy.Boards = await PaginatedList<Boards>.CreateAsync(boards, pageNumber ?? 1, pageSize);

            return View(boardHierarchy);

  
        }

        // GET: Boards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string URL = $"https://localhost:7244/api/Boards/{id}";
            HttpClient boardsClient = new HttpClient();
            var boards = await boardsClient.GetFromJsonAsync<Boards>(URL);

            if (boards == null)
            {
                return NotFound();
            }

            return View(boards);
        }

        // GET: Boards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Boards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Length,Width,Thickness,Volume,Type,Price,Equipment,Image")] Boards boards)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boards);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(boards);
        }

        // GET: Boards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Boards == null)
            {
                return NotFound();
            }

            var boards = await _context.Boards.FindAsync(id);
            if (boards == null)
            {
                return NotFound();
            }
            return View(boards);
        }

        // POST: Boards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Length,Width,Thickness,Volume,Type,Price,Equipment,Image")] Boards boards)
        {
            if (id != boards.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boards);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardsExists(boards.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(boards);
        }

        public async Task<IActionResult> Rent(int? id)
        {


            if (id == null)
            {
                return NotFound();
            }

            string URL = $"https://localhost:7244/api/Boards/{id}";
            HttpClient boardsClient = new HttpClient();
            var boards = await boardsClient.GetFromJsonAsync<Boards>(URL);

            var boardleasing = new BoardLeasing();
            boardleasing.Board = boards;
            if (boardleasing.Board == null)
            {
                return NotFound();
            }

            return View(boardleasing);
        }

        [HttpPost, ActionName("Rent")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RentConfirmed(int id, [Bind("TimeFrame")] Lease lease)
        {
            string URL = $"https://localhost:7244/api/Boards/Rent/{id}";
            HttpClient boardsClient = new HttpClient();

           
            var user = await userManager.GetUserAsync(User);
            lease.UserID = userManager.GetUserId(User);

            await boardsClient.PostAsJsonAsync<Lease>(URL,lease);  

            return RedirectToAction(nameof(Index));
        }


        // GET: Boards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Boards == null)
            {
                return NotFound();
            }

            var boards = await _context.Boards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boards == null)
            {
                return NotFound();
            }

            return View(boards);
        }

        // POST: Boards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Boards == null)
            {
                return Problem("Entity set 'SurfProjektContext.Boards'  is null.");
            }
            var boards = await _context.Boards.FindAsync(id);
            if (boards != null)
            {
                _context.Boards.Remove(boards);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoardsExists(int id)
        {
          return (_context.Boards?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
