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
    public class ContattiController : ControllerBase
    {
        private readonly RubricaContext _context;

        public ContattiController(RubricaContext context)
        {
            _context = context;
        }

        // GET: api/Contatti
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Contatto>>> GetContatti()
        {
            return await _context.Contatti.ToListAsync();
        }

        // GET: api/Contatti/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Contatto>> GetContatto(int id)
        {
            var contatto = await _context.Contatti.FindAsync(id);

            if (contatto == null)
            {
                return NotFound();
            }

            return contatto;
        }

        // PUT: api/Contatti/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContatto(int id, Contatto contatto)
        {
            if (id != contatto.Id)
            {
                return BadRequest();
            }

            _context.Entry(contatto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContattoExists(id))
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

        // POST: api/Contatti
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Contatto>> PostContatto(Contatto contatto)
        {
            _context.Contatti.Add(contatto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContatto", new { id = contatto.Id }, contatto);
        }

        // DELETE: api/Contatti/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContatto(int id)
        {
            var contatto = await _context.Contatti.FindAsync(id);
            if (contatto == null)
            {
                return NotFound();
            }

            var famigliari = await _context.Famigliari
                .Where(f => f.ContattoId == id || f.FamigliareId == id)
                .ToListAsync();

            _context.Famigliari.RemoveRange(famigliari);

            _context.Contatti.Remove(contatto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ContattoExists(int id)
        {
            return _context.Contatti.Any(e => e.Id == id);
        }
    }
}
