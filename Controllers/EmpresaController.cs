using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pricewhisper.Models;
using pricewhisper.Services;

namespace pricewhisper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly OracleDbContext _context;
        private readonly CNPJaService _cnpjaService;

        public EmpresaController(OracleDbContext dbContext, CNPJaService cnpjaService)
        {
            _context = dbContext;
            _cnpjaService = cnpjaService;
        }

        /// <summary>
        /// Obt�m todos as empresas.
        /// </summary>
        /// <remarks>
        /// Exemplo de uso: GET /api/Empresa
        /// </remarks>
        /// <response code="200">Retorna a lista de todas as empresas.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Empresa>> Get()
        {
            return _context.Empresas.Include(e => e.Usuarios).ToList();
        }

        /// <summary>
        /// Obt�m uma Empresa pelo ID.
        /// </summary>
        /// <param name="id">O ID da empresa que voc� deseja obter.</param>
        /// <returns>A empresa com o id especificado.</returns>
        /// <remarks>
        /// Exemplo de uso: GET /api/Empresa/1
        /// </remarks>
        /// <response code="200">Retorna a Empresa.</response>
        /// <response code="404">Empresa n�o encontrada.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Empresa?>> GetById(int id)
        {
            return await _context.Empresas
                .Include(e => e.Usuarios)
                .Where(x => x.EmpresaId == id)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Cria uma nova Empresa.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite que voc� crie uma nova Empresa. 
        /// 
        /// Exemplo de uso:
        /// 
        /// POST /api/Empresa
        /// 
        /// Body:
        /// {
        ///  "empresaId": 0,
        ///  "cnpj": "string",
        ///  "razaoSocial": "string",
        ///  "nomeFantasia": "string",
        ///  "usuarios": [
        ///    {
        ///      "usuarioId": 0,
        ///      "nome": "string",
        ///      "nomeUsuario": "string",
        ///      "senha": "string",
        ///      "empresaId": 0,
        ///      "empresa": "string"
        ///    }
        ///  ]
        /// }
        /// </remarks>
        /// <response code="201">A empresa foi criada com sucesso.</response>
        /// <response code="400">Solicita��o inv�lida. Pode ocorrer se algum campo obrigat�rio estiver faltando ou o formato estiver incorreto.</response>
        /// <response code="500">Erro interno do servidor. Ocorre se houver um problema ao processar a solicita��o.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Create(Empresa empresa)
        {
            var cnpjInfo = await _cnpjaService.ConsultarCNPJ(empresa.CNPJ);
            if (cnpjInfo == null || cnpjInfo.TaxId == null)
            {
                return BadRequest("CNPJ inv�lido ou n�o encontrado na base da Receita Federal");
            }

            await _context.Empresas.AddAsync(empresa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = empresa.EmpresaId }, empresa);
        }

        /// <summary>
        /// Atualiza uma Empresa existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite que voc� atualize uma empresa. 
        /// 
        /// Exemplo de uso:
        /// 
        /// PUT /api/Empresa
        /// 
        /// Body:
        /// {
        ///  "empresaId": 0,
        ///  "cnpj": "string",
        ///  "razaoSocial": "string",
        ///  "nomeFantasia": "string",
        ///  "usuarios": [
        ///    {
        ///      "usuarioId": 0,
        ///      "nome": "string",
        ///      "nomeUsuario": "string",
        ///      "senha": "string",
        ///      "empresaId": 0,
        ///      "empresa": "string"
        ///    }
        ///  ]
        /// }
        /// </remarks>
        /// <response code="200">A empresa foi atualizada com sucesso.</response>
        /// <response code="400">Solicita��o inv�lida. Pode ocorrer se algum campo obrigat�rio estiver faltando ou o formato estiver incorreto.</response>
        /// <response code="500">Erro interno do servidor. Ocorre se houver um problema ao processar a solicita��o.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update(Empresa empresa)
        {
            _context.Empresas.Update(empresa);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Remove uma empresa pelo ID.
        /// </summary>
        /// <param name="id">O ID da empresa que voc� deseja remover.</param>
        /// <remarks>
        /// Exemplo de uso: DELETE /api/Empresa/1
        /// </remarks>
        /// <response code="200">Empresa deletada com sucesso</response>
        /// <response code="404">Empresa n�o encontrada.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            var empresaGetByIdResult = await GetById(id);

            if (empresaGetByIdResult.Value is null) { return NotFound(); }

            _context.Empresas.Remove(empresaGetByIdResult.Value);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
