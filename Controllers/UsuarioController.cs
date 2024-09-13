using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pricewhisper.Models;

namespace pricewhisper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly OracleDbContext _context;

        public UsuarioController(OracleDbContext dbContext) => _context = dbContext;

        /// <summary>
        /// Obtém todos os usuários.
        /// </summary>
        /// <remarks>
        /// Exemplo de uso: GET /api/Usuario
        /// </remarks>
        /// <response code="200">Retorna a lista de todos os usuários.</response>
        [HttpGet]
        public ActionResult<IEnumerable<Usuario>> Get()
        {
            return _context.Usuarios;
        }

        /// <summary>
        /// Obtém um usuário pelo ID.
        /// </summary>
        /// <param name="id">O ID do usuário que você deseja obter.</param>
        /// <returns>O usuário com o ID especificado.</returns>
        /// /// <remarks>
        /// Exemplo de uso: GET /api/Usuario/1
        /// </remarks>
        /// <response code="200">Retorna o usuário.</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario?>> GetById(int id)
        {
            return await _context.Usuarios.Where(x => x.UsuarioId == id).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Cria um novo Usuário.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite que você crie um novo usuário. 
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
        /// <response code="201">O usuário foi criado com sucesso.</response>
        /// <response code="400">Solicitação inválida. Pode ocorrer se algum campo obrigatório estiver faltando ou o formato estiver incorreto.</response>
        /// <response code="500">Erro interno do servidor. Ocorre se houver um problema ao processar a solicitação.</response>
        [HttpPost]
        public async Task<ActionResult> Create(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = usuario.UsuarioId}, usuario);
        }

        /// <summary>
        /// Atualiza um Usuário existente.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite que você atualize um usuário. 
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
        /// <response code="200">O usuário foi atualizado com sucesso.</response>
        /// <response code="400">Solicitação inválida. Pode ocorrer se algum campo obrigatório estiver faltando ou o formato estiver incorreto.</response>
        /// <response code="500">Erro interno do servidor. Ocorre se houver um problema ao processar a solicitação.</response>
        [HttpPut]
        public async Task<ActionResult> Update(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok();
        }


        /// <summary>
        /// Remove um usuário pelo ID.
        /// </summary>
        /// <param name="id">O ID do usuário que você deseja remover.</param>
        /// <remarks>
        /// Exemplo de uso: DELETE /api/Usuario/1
        /// </remarks>
        /// <response code="200">Usuário deletado com sucesso</response>
        /// <response code="404">Usuário não encontrado.</response>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var usuarioGetByIdResult = await GetById(id);

            if (usuarioGetByIdResult.Value is null) { return NotFound(); }

            _context.Usuarios.Remove(usuarioGetByIdResult.Value);
            return Ok();
        }
    }
}
