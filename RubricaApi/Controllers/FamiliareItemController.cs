using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RubricaApi.Model;
using RubricaApi.Models;

namespace RubricaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FamiliareItemController : ControllerBase
    {
        private readonly RubricaContext _context;

        public FamiliareItemController(RubricaContext context)
        {
            _context = context;
        }

        // GET: api/FamiliareItem
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Familiare>>> GetFamiliari()
        {
            return await _context.Familiari.ToListAsync();
        }

        // GET: api/FamiliareItem/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Familiare>> GetFamiliare(long id)
        {
            var familiare = await _context.Familiari.FindAsync(id);

            if (familiare == null)
            {
                return NotFound();
            }

            return familiare;
        }

        // PUT: api/FamiliareItem/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFamiliare(long id, Familiare familiare)
        {
            if (id != familiare.FamiliareId)
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
                if (!FamiliareExists(id))
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

        // POST: api/FamiliareItem
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Familiare>> PostFamiliare(Familiare familiare)
        {
            _context.Familiari.Add(familiare);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFamiliare", new { id = familiare.FamiliareId }, familiare);
        }

        // DELETE: api/FamiliareItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFamiliare(long id)
        {
            var familiare = await _context.Familiari.FindAsync(id);
            if (familiare == null)
            {
                return NotFound();
            }

            _context.Familiari.Remove(familiare);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FamiliareExists(long id)
        {
            return _context.Familiari.Any(e => e.FamiliareId == id);
        }
    }
}
