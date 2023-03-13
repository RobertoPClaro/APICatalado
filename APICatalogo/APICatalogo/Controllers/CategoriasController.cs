using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [EnableCors("PermitirApiRequest")] 
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapper;
        public CategoriasController(IUnitOfWork uof, IMapper mapper)
        {
            _uof = uof;
            _mapper = mapper;
        }

        [HttpGet("produto")]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasProdutos()
        {
            try
            {
                var categoriaProdutos = await _uof.CategoriaRepository.GetCategoriasProdutos();
                var categoriaProdutosDto = _mapper.Map<List<CategoriaDTO>>(categoriaProdutos);
                return Ok(categoriaProdutosDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Occoreu um problema ao tratar sua situação");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
        {
            try
            {
                var categorias = await _uof.CategoriaRepository.Get().ToListAsync();
                var categoriasDto = _mapper.Map<List<CategoriaDTO>>(categorias);

                return categoriasDto;
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public async Task <ActionResult<CategoriaDTO>> Get(int id)
        {
            try
            {
                var categoria = _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);
                if (categoria is null)
                {
                    return NotFound("Categoria com id = {id} não localizado...");
                }
                var categoriaDto = _mapper.Map<CategoriaDTO>(categoria);
                return Ok(categoriaDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Occoreu um problema ao tratar sua situação");
            }

        }
        [HttpPost]
        public async Task<ActionResult> Post(CategoriaDTO categoriaDto)
        {
            try
            {
                var categoria = _mapper.Map<Categoria>(categoriaDto);
                if (categoria is null)
                {
                    return BadRequest("Dados invalidos");
                }

                _uof.CategoriaRepository.Add(categoria);
                await _uof.Commit();

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

                return new CreatedAtRouteResult("ObterCategoria",
                    new { id = categoriaDTO.CategoriaId }, categoriaDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Occoreu um problema ao tratar sua situação");
            }

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, CategoriaDTO categoriaDto)
        {
            try
            {
                var categoria = _mapper.Map<Categoria>(categoriaDto);
                if (id != categoria.CategoriaId)
                {
                    return BadRequest("Dados invalidos");
                }

                _uof.CategoriaRepository.Update(categoria);
                await _uof.Commit();

                var categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

                return Ok(categoriaDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Occoreu um problema ao tratar sua situação");
            }

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var categoria = await _uof.CategoriaRepository.GetById(p => p.CategoriaId == id);

                if (categoria is null)
                {
                    return NotFound($"Categoria com id = {id} não localizado...");
                }
                _uof.CategoriaRepository.Delete(categoria);
                await _uof.Commit();

                return Ok(categoria);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Occoreu um problema ao tratar sua situação");
            }
        }
    }
}
