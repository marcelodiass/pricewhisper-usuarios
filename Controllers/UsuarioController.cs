using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pricewhisper.Models;
using pricewhisper.Models.DTOs;

namespace pricewhisper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly OracleDbContext _context;

        public UsuarioController(OracleDbContext dbContext) => _context = dbContext;

        /// <summary>
        /// Obt�m todos os usu�rios.
        /// </summary>
        /// <remarks>
        /// Exemplo de uso: GET /api/Usuario
        /// </remarks>
        /// <response code="200">Retorna a lista de todos os usu�rios.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<UsuarioDto>> Get()
        {
            var usuarios = _context.Usuarios
                .Include(u => u.Empresa)
                .Select(u => new UsuarioDto
                {
                    UsuarioId = u.UsuarioId,
                    Nome = u.Nome,
                    NomeUsuario = u.NomeUsuario,
                    Senha = u.Senha,
                    EmpresaId = u.EmpresaId,
                    RazaoSocialEmpresa = u.Empresa.RazaoSocial
                }).ToList();

            return usuarios;
        }

        /// <summary>
        /// Obt�m um usu�rio pelo ID.
        /// </summary>
        /// <param name="id">O ID do usu�rio que voc� deseja obter.</param>
        /// <returns>O usu�rio com o ID especificado.</returns>
        /// /// <remarks>
        /// Exemplo de uso: GET /api/Usuario/1
        /// </remarks>
        /// <response code="200">Retorna o usu�rio.</response>
        /// <response code="404">Usu�rio n�o encontrado.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UsuarioDto>> GetById(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Empresa)
                .Where(u => u.UsuarioId == id)
                .Select(u => new UsuarioDto
                {
                    UsuarioId = u.UsuarioId,
                    Nome = u.Nome,
                    NomeUsuario = u.NomeUsuario,
                    Senha = u.Senha,
                    EmpresaId = u.EmpresaId,
                    RazaoSocialEmpresa = u.Empresa.RazaoSocial
                })
                .SingleOrDefaultAsync();

            if (usuario == null) return NotFound();

            return usuario;
        }

        /// <summary>
        /// Cria um novo Usu�rio.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite que voc� crie um novo usu�rio. 
        /// 
        /// Exemplo de uso:
        /// 
        /// POST /api/Usuario
        /// 
        /// Body:
        /// {
        ///      "usuarioId": 0,
        ///      "nome": "string",
        ///      "nomeUsuario": "string",
        ///       "senha": "string",
        ///      "empresaId": 0,
        ///      "empresa": {
        ///         "empresaId": 0,
        ///         "cnpj": "string",
        ///         "razaoSocial": "string",
        ///         "nomeFantasia": "string",
        ///         "usuarios": [
        ///             "string"
        ///         ]
        ///      }
        /// }
        /// </remarks>
        /// <response code="201">O usu�rio foi criado com sucesso.</response>
        /// <response code="400">Solicita��o inv�lida. Pode ocorrer se algum campo obrigat�rio estiver faltando ou o formato estiver incorreto.</response>
        /// <response code="500">Erro interno do servidor. Ocorre se houver um problema ao processar a solicita��o.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Create(Usuario usuario)
        {
            var empresa = await _context.Empresas.FindAsync(usuario.EmpresaId);
            if (empresa == null)
            {
                return BadRequest("Empresa n�o encontrada");
            }

            usuario.Empresa = empresa;
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = usuario.UsuarioId }, usuario);
        }

        /// <summary>
        /// Atualiza um Usu�rio existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite que voc� atualize um usu�rio. 
        /// 
        /// Exemplo de uso:
        /// 
        /// PUT /api/Usuario
        /// 
        /// Body:
        /// {
        ///      "usuarioId": 0,
        ///      "nome": "string",
        ///      "nomeUsuario": "string",
        ///       "senha": "string",
        ///      "empresaId": 0,
        ///      "empresa": {
        ///         "empresaId": 0,
        ///         "cnpj": "string",
        ///         "razaoSocial": "string",
        ///         "nomeFantasia": "string",
        ///         "usuarios": [
        ///             "string"
        ///         ]
        ///      }
        /// }
        /// </remarks>
        /// <response code="200">O usu�rio foi atualizado com sucesso.</response>
        /// <response code="400">Solicita��o inv�lida. Pode ocorrer se algum campo obrigat�rio estiver faltando ou o formato estiver incorreto.</response>
        /// <response code="500">Erro interno do servidor. Ocorre se houver um problema ao processar a solicita��o.</response>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Update(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok();
        }


        /// <summary>
        /// Remove um usu�rio pelo ID.
        /// </summary>
        /// <param name="id">O ID do usu�rio que voc� deseja remover.</param>
        /// <remarks>
        /// Exemplo de uso: DELETE /api/Usuario/1
        /// </remarks>
        /// <response code="200">Usu�rio deletado com sucesso</response>
        /// <response code="404">Usu�rio n�o encontrado.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            // Recupera a entidade Usuario diretamente do banco de dados
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
