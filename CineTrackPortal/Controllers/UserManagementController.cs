using CineTrackPortal.Data;
using CineTrackPortal.Models;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CineTrackPortal.Controllers
{
    [Authorize]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 10;

        public UserManagementController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        // GET: UserManagementController/List
        public async Task<IActionResult> ListUsers(string searchTerm, int pageIndex = 1)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(m => 
                                m.FirstName.Contains(searchTerm) || 
                                m.LastName.Contains(searchTerm));
            }

            var users = await PaginatedList<ExtendedUserModel>.CreateAsync(
                query.OrderBy(m => m.FirstName), pageIndex, 10);

            ViewBag.SearchTerm = searchTerm;

            return View(users);
        }


        [HttpGet]
        public async Task<IActionResult> SearchUsers(string searchTerm, int pageIndex = 1)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(m =>
                                m.FirstName.Contains(searchTerm) ||
                                m.LastName.Contains(searchTerm));
            }

            var users = await PaginatedList<ExtendedUserModel>.CreateAsync(
                            query.OrderBy(m => m.FirstName), pageIndex, 10);

            ViewBag.SearchTerm = searchTerm;

            return PartialView("_UsersTablePartial", users);
        }


        // GET: UserManagementController/Details/{Guid}
        public async Task<IActionResult> Details(Guid id)
        {
            var user= await _context.Users.FirstOrDefaultAsync(a => a.Id == id.ToString());

            if (user == null)
                return NotFound();

            return View(user);
        }


        // GET: UserManagementController/Create
        public IActionResult Create()
        {
            return View();
        }


        // POST: UserManagementController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExtendedUserModel user)
        {
            if (string.IsNullOrWhiteSpace(user.FirstName))
                ModelState.AddModelError("FirstName", "First name is required.");
            
            if (string.IsNullOrWhiteSpace(user.LastName))
                ModelState.AddModelError("LastName", "Last name is required.");
            
            if (string.IsNullOrWhiteSpace(user.Email))
                ModelState.AddModelError("Email", "Email is required.");

            if (ModelState.IsValid)
            {
                // Check for existing user by Email (case-insensitive)
                var exists = await _context.Users
                .AnyAsync(m => m.Email.ToLower() == user.Email.ToLower());

                if (exists)
                {
                    ModelState.AddModelError("Title", "A user with this email already exists.");
                    return View(user);
                }

                // Generate a random username if not provided
                if (string.IsNullOrWhiteSpace(user.UserName))
                {
                    string baseUserName = $"{user.FirstName}{user.LastName}".ToLower();
                    string randomUserName;
                    var rand = new Random();
                    do
                    {
                        randomUserName = baseUserName + rand.Next(1000, 9999);
                    }
                    while (await _context.Users.AnyAsync(u => u.UserName == randomUserName));
                    user.UserName = randomUserName;
                }

                user.Id = Guid.NewGuid().ToString();
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                ViewBag.EditSuccess = true;
                return View(user); // Return the same view to show the alert
            }

            return View(user);
        }



        // GET: UserManagementController/Edit/{Guid}
        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _context.Users.FindAsync(id.ToString());
            if (user == null)
                return NotFound();
            return View(user);
        }


        // POST: UserManagementController/Edit/{Guid}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id, FirstName, LastName, Email, UserName")] ExtendedUserModel user)
        {
            if (id.ToString() != user.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(user);

            // Only update Id, FirstName, LastName, Email, UserName, not navigation properties
            var userToUpdate = await _context.Users.FindAsync(id.ToString());
            if (userToUpdate == null)
                return NotFound();

            userToUpdate.FirstName = user.FirstName;
            userToUpdate.LastName = user.LastName;
            userToUpdate.Email = user.Email;
            userToUpdate.UserName = user.UserName;

            try
            {
                await _context.SaveChangesAsync();
                ViewBag.EditSuccess = true;
                return View(userToUpdate); // Return the view with the updated model and success flag
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Users.Any(e => e.Id == id.ToString()))
                    return NotFound();
                throw;
            }
        }


        // GET: UserManagementController/Delete/Guid
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FindAsync(id.ToString());
            if (user == null)
                return NotFound();
            return View(user);
        }


        // POST: UserManagementController/Delete/Guid
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id.ToString());

            if (user == null)
                return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            ViewBag.EditSuccess = true;
            return View(user);
        }

    }
}
