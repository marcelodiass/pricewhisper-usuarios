using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pricewhisper.Models;
using pricewhisper.Services;
using pricewhisper.Models.DTOs;

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
        /// Obtém todos as empresas.
        /// </summary>
        /// <remarks>
        /// Exemplo de uso: GET /api/Empresa
        /// </remarks>
        /// <response code="200">Retorna a lista de todas as empresas.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<EmpresaDto>> Get()
        {
            var empresas = _context.Empresas
                .Include(e => e.Usuarios)
                .Select(e => new EmpresaDto
                {
                    EmpresaId = e.EmpresaId,
                    CNPJ = e.CNPJ,
                    RazaoSocial = e.RazaoSocial,
                    NomeFantasia = e.NomeFantasia,
                    Usuarios = e.Usuarios.Select(u => new UsuarioDto
                    {
                        UsuarioId = u.UsuarioId,
                        Nome = u.Nome,
                        NomeUsuario = u.NomeUsuario,
                        Senha = u.Senha,
                        EmpresaId = u.EmpresaId,
                        RazaoSocialEmpresa = e.RazaoSocial
                    }).ToList()
                }).ToList();

            return empresas;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmpresaDto>> GetById(int id)
        {
            var empresa = await _context.Empresas
                .Include(e => e.Usuarios)
                .Where(e => e.EmpresaId == id)
                .Select(e => new EmpresaDto
                {
                    EmpresaId = e.EmpresaId,
                    CNPJ = e.CNPJ,
                    RazaoSocial = e.RazaoSocial,
                    NomeFantasia = e.NomeFantasia,
                    Usuarios = e.Usuarios.Select(u => new UsuarioDto
                    {
                        UsuarioId = u.UsuarioId,
                        Nome = u.Nome,
                        NomeUsuario = u.NomeUsuario,
                        Senha = u.Senha,
                        EmpresaId = u.EmpresaId,
                        RazaoSocialEmpresa = e.RazaoSocial
                    }).ToList()
                })
                .SingleOrDefaultAsync();

            if (empresa == null) return NotFound();

            return empresa;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Create(Empresa empresa)
        {
            var cnpjInfo = await _cnpjaService.ConsultarCNPJ(empresa.CNPJ);
            if (cnpjInfo == null || cnpjInfo.TaxId == null)
            {
                return BadRequest("CNPJ inválido ou não encontrado na base da Receita Federal");
            }

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
        /// PUT /api/Empresa/{id}
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
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update(int id, Empresa empresa)
        {
            if (id != empresa.EmpresaId)
            {
                return BadRequest("ID da empresa não corresponde ao ID na URL.");
            }

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            // Recupera a entidade Empresa diretamente do banco de dados
            var empresa = await _context.Empresas
                .Include(e => e.Usuarios) // Opcional: Inclui usuários relacionados, se necessário
                .FirstOrDefaultAsync(e => e.EmpresaId == id);

            if (empresa == null)
            {
                return NotFound();
            }

            // Opcional: Remover ou reatribuir usuários relacionados antes de deletar a empresa
            if (empresa.Usuarios != null && empresa.Usuarios.Any())
            {
                _context.Usuarios.RemoveRange(empresa.Usuarios);
            }

            _context.Empresas.Remove(empresa);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
