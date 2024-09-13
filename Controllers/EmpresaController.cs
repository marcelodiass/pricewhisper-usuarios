using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pricewhisper.Models;

namespace pricewhisper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly OracleDbContext _context;

        public EmpresaController(OracleDbContext dbContext) => _context = dbContext;

        /// <summary>
        /// Obtém todos as empresas.
        /// </summary>
        /// <remarks>
        /// Exemplo de uso: GET /api/Empresa
        /// </remarks>
        /// <response code="200">Retorna a lista de todas as empresas.</response>
        [HttpGet]
        public ActionResult<IEnumerable<Empresa>> Get()
        {
            return _context.Empresas;
        }

        /// <summary>
        /// Obtém uma Empresa pelo ID.
        /// </summary>
        /// <param name="id">O ID da empresa que você deseja obter.</param>
        /// <returns>A empresa com o id especificado.</returns>
        /// <remarks>
        /// Exemplo de uso: GET /api/Empresa/1
        /// </remarks>
        /// <response code="200">Retorna a Empresa.</response>
        /// <response code="404">Empresa não encontrada.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Empresa?>> GetById(int id)
        {
            return await _context.Empresas.Where(x => x.EmpresaId == id).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Cria uma nova Empresa.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite que você crie uma nova Empresa. 
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
        /// <response code="400">Solicitação inválida. Pode ocorrer se algum campo obrigatório estiver faltando ou o formato estiver incorreto.</response>
        /// <response code="500">Erro interno do servidor. Ocorre se houver um problema ao processar a solicitação.</response>
        [HttpPost]
        public async Task<ActionResult> Create(Empresa empresa)
        {
            await _context.Empresas.AddAsync(empresa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = empresa.EmpresaId }, empresa);
        }

        /// <summary>
        /// Atualiza uma Empresa existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite que você atualize uma empresa. 
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
        /// <response code="400">Solicitação inválida. Pode ocorrer se algum campo obrigatório estiver faltando ou o formato estiver incorreto.</response>
        /// <response code="500">Erro interno do servidor. Ocorre se houver um problema ao processar a solicitação.</response>
        [HttpPut]
        public async Task<ActionResult> Update(Empresa empresa)
        {
            _context.Empresas.Update(empresa);
            await _context.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Remove uma empresa pelo ID.
        /// </summary>
        /// <param name="id">O ID da empresa que você deseja remover.</param>
        /// <remarks>
        /// Exemplo de uso: DELETE /api/Empresa/1
        /// </remarks>
        /// <response code="200">Empresa deletada com sucesso</response>
        /// <response code="404">Empresa não encontrada.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var empresaGetByIdResult = await GetById(id);

            if (empresaGetByIdResult.Value is null) { return NotFound(); }

            _context.Empresas.Remove(empresaGetByIdResult.Value);
            return Ok();
        }
    }
}
