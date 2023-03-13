using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly IMapper _mapeer;

        public ProdutosController(IUnitOfWork context, IMapper mapeer)
        {
            _uof = context;
            _mapeer = mapeer;
        }

        [HttpGet("menorpreco")]
        public async Task< ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosPrecos()
        {
            var produtos = await _uof.ProdutoRepository.GetProdutosPorPrecos();
            var produtosDto = _mapeer.Map<List<ProdutoDTO>>(produtos);

            return produtosDto;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            try
            {
                var produtos = await _uof.ProdutoRepository.GetProdutos(produtosParameters); 

                var metadata = new
                {
                    produtos.TotalCount,
                    produtos.PageSize,
                    produtos.CurrentPage,
                    produtos.TotalPages,
                    produtos.HasNext,
                    produtos.HasPrevious 
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

                if(produtos == null)
                {
                   return NotFound("Produtos não encontrados...");
                }
                var produtosDto = _mapeer.Map<List<ProdutoDTO>>(produtos);

                return produtosDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Occoreu um problema ao tratar sua situação");
            }
        }

        [HttpGet("{id:int:min(1)}", Name="ObterProduto")]
        public async Task< ActionResult<ProdutoDTO>> Get(int id)
        {
            try
            {
                var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);
                if (produto is null)
                {
                    return NotFound("Produto não encontrado...");
                }
                var produtoDto = _mapeer.Map<ProdutoDTO>(produto);
                return produtoDto;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Occoreu um problema ao tratar sua situação");
            }
        }

        [HttpPost]
        public async Task< ActionResult> Post(ProdutoDTO produtoDto)
        {
            try
            {
                Produto produto = _mapeer.Map<Produto>(produtoDto);

                if (produto is null)
                {
                    return BadRequest();
                }

                _uof.ProdutoRepository.Add(produto);
                await _uof.Commit();

                ProdutoDTO produtoDTO = _mapeer.Map<ProdutoDTO>(produto);

                return new CreatedAtRouteResult("ObterProduto",
                    new { id = produtoDTO.ProdutoId }, produtoDTO);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Occoreu um problema ao tratar sua situação");
            }
        }

        [HttpPut("{id:int}")]
        public async Task <ActionResult> Put(int id, ProdutoDTO produtoDto)
        {
            try
            {
                if (id != produtoDto.ProdutoId)
                {
                    return BadRequest();
                }

                var produto = _mapeer.Map<Produto>(produtoDto);

                _uof.ProdutoRepository.Update(produto);
                await _uof.Commit();

                return Ok(produto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Occoreu um problema ao tratar sua situação");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task <ActionResult> Delete(int id)
        {
            try
            {
                var produto = await _uof.ProdutoRepository.GetById(p => p.ProdutoId == id);

                if (produto is null)
                {
                    return NotFound("Produto não localizado...");
                }
                _uof.ProdutoRepository.Delete(produto);
                await _uof.Commit();

                var produtoDto = _mapeer.Map<ProdutoDTO>(produto);

                return Ok(produtoDto);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Occoreu um problema ao tratar sua situação");
            }
        }
    }
}
