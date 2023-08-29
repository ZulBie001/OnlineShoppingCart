using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingCart.Data;
using OnlineShoppingCart.Models;

namespace OnlineShoppingCart.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppUsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AppUsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AppUsers
        //[HttpGet]
        public ActionResult<IEnumerable<dynamic>> GetUsers()
        {
            return _context.Users.Select(m => new
            {
                m.Id,
                m.Name,
                m.Email,
                Roles = m.Roles.Select(r => r.Name)
            }).ToList();
        }

        // GET: api/AppUsers/5
        public ActionResult<dynamic> GetAppUser()
        {
            var products = _context.Products
                //.Where(m => string.IsNullOrEmpty(categoryId) || m.CategoryId == categoryId)
                .Select(m => new ProductViewModel
                {
                    Brand = m.Brand.Name,
                    Name = m.Name,
                    Category = m.Category.Name,
                    Description = m.Description,
                    Price = m.Price,
                    Slug = m.Slug,
                    ReleaseDate = m.ReleaseDate,
                    Id = m.Id,
                    Stock = m.Stock,
                    ImageUrl = m.Images.OrderBy(n => n.DbEntryTime).Select(n => n.URL).FirstOrDefault()
                }).ToList();

            return products;
        }

        // PUT: api/AppUsers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppUser(string id, AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return BadRequest();
            }

            _context.Entry(appUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppUserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/AppUsers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AppUser>> PostAppUser(AppUser appUser)
        {
            _context.Users.Add(appUser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AppUserExists(appUser.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAppUser", new { id = appUser.Id }, appUser);
        }

        // DELETE: api/AppUsers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppUser(string id)
        {
            var appUser = await _context.Users.FindAsync(id);
            if (appUser == null)
            {
                return NotFound();
            }

            _context.Users.Remove(appUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppUserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
