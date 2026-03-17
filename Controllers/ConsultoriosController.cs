using ConsultorioApi.Data;
using ConsultorioApi.Models;
using ConsultorioApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConsultorioApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsultoriosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ViaCepService _viaCepService;

        public ConsultoriosController(AppDbContext context, ViaCepService viaCepService)
        {
            _context = context;
            _viaCepService = viaCepService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Models.Consultorio>>> GetConsultorios()
        {
            var consultorios = await _context.Consultorios.ToListAsync();
            return consultorios;
        }

        [HttpPost]
        public async Task<ActionResult<Models.Consultorio>> PostConsultorio(Models.Consultorio consultorio)
        {
            var endereco = await _viaCepService.BuscarEnderecoAsync(consultorio.Cep);

            if (endereco != null)
            {
                consultorio.Logradouro = endereco.logradouro;
                consultorio.Bairro = endereco.bairro;
                consultorio.Localidade = endereco.localidade;
                consultorio.Uf = endereco.uf;
            }

            _context.Consultorios.Add(consultorio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetConsultorio), new { id = consultorio.Id }, consultorio);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Consultorio>> GetConsultorio(int id)
        {
            var consultorio = await _context.Consultorios.FindAsync(id);

            if (consultorio == null)
            {
                return NotFound();
            }
            
            return consultorio;
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Models.Consultorio>> DeleteConsultorio(int id)
        {
            var consultorio = await _context.Consultorios.FindAsync(id);
            if (consultorio == null) return NotFound();
            _context.Remove(consultorio);
            await _context.SaveChangesAsync();
            return Ok(consultorio);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Models.Consultorio>> UpdateConsultorio(int id, Models.Consultorio consultorio)
        {
            var consultorioAntigo = await _context.Consultorios.FindAsync(id);
            if (consultorioAntigo == null || consultorio.Id != consultorioAntigo.Id) return NotFound();

            consultorioAntigo.Nome = consultorio.Nome;
            consultorioAntigo.Cep = consultorio.Cep;
            consultorioAntigo.Numero = consultorio.Numero;

            _context.Update(consultorioAntigo);
            await _context.SaveChangesAsync();
            return Ok(consultorioAntigo);
        }
    }
}
