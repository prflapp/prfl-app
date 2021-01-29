using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Profile.Server.Data;
using Profile.Shared.Models;

namespace Profile.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly ProfileContext _context;
        private readonly UserManager<ProfileUser> _userManager;

        public FavoriteController(ProfileContext context, UserManager<ProfileUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Favorite
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetFavorite()
        {
            var userId = _userManager.GetUserId(User);
            return await _context.Favorite.Where(f => f.ProfileUserId == userId).ToListAsync();
        }

        // GET: api/Account/u/{username}
        [HttpGet("u/{username}")]
        public async Task<ActionResult<IEnumerable<Favorite>>> GetFavoriteByUsername(string username)
        {
            //var user = await _userManager.FindByNameAsync(username);
            return await _context.Favorite.Include(f => f.ProfileUser).Where(a => a.ProfileUser.UserName == username).ToListAsync();
        }

        // GET: api/Favorite/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Favorite>> GetFavorite(string id)
        {
            var favorite = await _context.Favorite.FindAsync(id);

            if (favorite == null)
            {
                return NotFound();
            }

            return favorite;
        }
        
        // GET: api/Favorite/project/5
        [HttpGet("{linkType}/{id}")]
        public async Task<ActionResult<Favorite>> GetFavorite(string id, LinkType linkType)
        {
            var favorite = await _context.Favorite.Where(f => f.Type == linkType).FirstOrDefaultAsync(f => f.LinkId == id);

            if (favorite == null)
            {
                return NotFound();
            }

            return favorite;
        }

        // PUT: api/Favorite/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFavorite(string id, Favorite favorite)
        {
            if (id != favorite.FavoriteId)
            {
                return BadRequest();
            }

            _context.Entry(favorite).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FavoriteExists(id))
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

        // POST: api/Favorite
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Favorite>> PostFavorite(Favorite favorite)
        {
            _context.Favorite.Add(favorite);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFavorite", new { id = favorite.FavoriteId }, favorite);
        }

        // DELETE: api/Favorite/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite(string id)
        {
            var favorite = await _context.Favorite.FindAsync(id);

            if(favorite.Type == LinkType.Account) {
                var link = await _context.Account.FirstOrDefaultAsync(l => l.AccountId == favorite.LinkId);
                link.IsFavorite = false;
            }
            else if(favorite.Type == LinkType.Link) {
                var link = await _context.Link.FirstOrDefaultAsync(l => l.LinkId == favorite.LinkId);
                link.IsFavorite = false;
            }
            else if(favorite.Type == LinkType.Project) {
                var link = await _context.Project.FirstOrDefaultAsync(l => l.ProjectId == favorite.LinkId);
                link.IsFavorite = false;
            }
            else if(favorite.Type == LinkType.Video) {
                var link = await _context.Video.FirstOrDefaultAsync(l => l.VideoId == favorite.LinkId);
                link.IsFavorite = false;
            }


            if (favorite == null)
            {
                return NotFound();
            }

            _context.Favorite.Remove(favorite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FavoriteExists(string id)
        {
            return _context.Favorite.Any(e => e.FavoriteId == id);
        }


    }
}