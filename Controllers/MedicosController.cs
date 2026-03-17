using ConsultorioApi.Data;
using ConsultorioApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Consultorio.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MedicosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Medico>>> GetMedico()
        {
            var pacientes = await _context.Medicos.Include(m => m.Consultorio).ToListAsync();
            return Ok(pacientes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<Medico>>> GetMedico(int id)
        {
            var medico = await _context.Medicos.Include(m => m.Consultorio).FirstOrDefaultAsync(m => m.Id == id);
            if (medico == null) NotFound();
            return Ok(medico);
        }

        [HttpPost]
        public async Task<ActionResult<Medico>> PostMedico(Medico medico)
        {
            var consultorio = await _context.Consultorios.FindAsync(medico.ConsultorioId);
            if (consultorio == null) return NotFound();
            medico.Consultorio = consultorio;
            await _context.AddAsync(medico);
            await _context.SaveChangesAsync();
            return Ok(medico);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Medico>> DeleteMedico(int id)
        {
            var medico = await _context.Medicos.FindAsync(id);
            if (medico == null) NotFound();
            _context.Remove(medico);
            await _context.SaveChangesAsync();
            return Ok(medico);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Medico>> UpdateMedico(Medico medico, int id)
        {
            var medicoAntigo = await _context.Medicos.FindAsync(id);
            if (medicoAntigo == null || medico.Id != medicoAntigo.Id) return NotFound();

            var consultorio = await _context.Consultorios.FindAsync(medico.ConsultorioId);
            medicoAntigo.Consultorio = consultorio;

            medicoAntigo.Name = medico.Name;
            medicoAntigo.Crm = medico.Crm;
            medicoAntigo.ConsultorioId = medico.ConsultorioId;

            _context.Medicos.Update(medicoAntigo);
            await _context.SaveChangesAsync();
            return Ok(medicoAntigo);

        }
    }
}