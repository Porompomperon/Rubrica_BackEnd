using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RubricaApi.Data;
using RubricaApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace RubricaApi.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FamigliariController : ControllerBase
    {
        private readonly RubricaContext _context;

        public FamigliariController(RubricaContext context)
        {
            _context = context;
        }

        // GET: api/Famigliari/5
        [HttpGet("{contattoId}")]
        public async Task<ActionResult<IEnumerable<Famigliare>>> GetFamigliari(int contattoId)
        {
            return await _context.Famigliari
                .Where(f => f.ContattoId == contattoId)
                .ToListAsync();
        }

        // GET: api/Famigliari/5/3
        [HttpGet("{contattoId}/{famigliareId}")]
        public async Task<ActionResult<Famigliare>> GetFamigliare(int contattoId, int famigliareId)
        {
            var famigliare = await _context.Famigliari.FindAsync(contattoId, famigliareId);

            if (famigliare == null)
            {
                return NotFound();
            }

            return famigliare;
        }

        // PUT: api/Famigliari/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{contattoId}/{famigliareId}")]
        public async Task<IActionResult> PutFamigliare(int contattoId, int famigliareId, Famigliare famigliare)
        {
            if (contattoId != famigliare.ContattoId || famigliareId != famigliare.FamigliareId)
            {
                return BadRequest();
            }

            _context.Entry(famigliare).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FamigliareExists(contattoId, famigliareId))
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

        // POST: api/Famigliari
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Famigliare>> PostFamigliare(Famigliare famigliare)
        {
            _context.Famigliari.Add(famigliare);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FamigliareExists(famigliare.ContattoId, famigliare.FamigliareId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFamigliare", new { contattoId = famigliare.ContattoId, famigliareId = famigliare.FamigliareId }, famigliare);
        }

        // DELETE: api/Famigliari/5
        [HttpDelete("{contattoId}/{famigliareId}")]
        public async Task<IActionResult> DeleteFamigliare(int contattoId, int famigliareId)
        {
            var famigliare = await _context.Famigliari.FindAsync(contattoId, famigliareId);
            if (famigliare == null)
            {
                return NotFound();
            }

            _context.Famigliari.Remove(famigliare);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FamigliareExists(int contattoId, int famigliareId)
        {
            return _context.Famigliari.Any(e => e.ContattoId == contattoId && e.FamigliareId == famigliareId);
        }
    }
}
