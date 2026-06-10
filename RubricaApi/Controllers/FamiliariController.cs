using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RubricaApi.Data;
using RubricaApi.Models;

namespace RubricaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamiliariController : ControllerBase
    {
        private readonly RubricaContext _context;

        public FamiliariController(RubricaContext context)
        {
            _context = context;
        }

        // GET: api/Familiari
        [HttpGet("{contattoId}")]
        public async Task<ActionResult<IEnumerable<Familiare>>> GetFamiliari(long contattoId)
        {
            return await _context.Familiari.ToListAsync();
        }
        // GET: api/Familiari/5
        [HttpGet("{contattoId}/{familiareId}")]
        public async Task<ActionResult<Familiare>> GetFamiliare(long contattoId, long familiareId)
        {
            var familiare = await _context.Familiari.FindAsync(contattoId, familiareId);

            if (familiare == null)
            {
                return NotFound();
            }

            return familiare;
        }

        // PUT: api/Familiari/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{contattoId}/{familiareId}")]
        public async Task<IActionResult> PutFamiliare(long contattoId, long familiareId, Familiare familiare)
        {
            if (contattoId != familiare.ContattoId || familiareId != familiare.FamiliareId)
            {
                return BadRequest();
            }

            _context.Entry(familiare).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FamiliareExists(contattoId))
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

        // POST: api/Familiari
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Familiare>> PostFamiliare(Familiare familiare)
        {
            _context.Familiari.Add(familiare);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (FamiliareExists(familiare.ContattoId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetFamiliare", new { contattoId = familiare.ContattoId, familiareId = familiare.FamiliareId }, familiare);
        }

        // DELETE: api/Familiari/5
        [HttpDelete("{contattoId}/{familiareId}")]
        public async Task<IActionResult> DeleteFamiliare(long contattoId, long familiareId)
        {
            var familiare = await _context.Familiari.FindAsync(contattoId, familiareId);
            if (familiare == null)
            {
                return NotFound();
            }

            _context.Familiari.Remove(familiare);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FamiliareExists(long contattoId)
        {
            return _context.Familiari.Any(e => e.ContattoId == contattoId);
        }
    }
}
