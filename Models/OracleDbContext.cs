using Microsoft.EntityFrameworkCore;

namespace pricewhisper.Models
{
    public class OracleDbContext : DbContext
    {
        public OracleDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Empresa> Empresas { get; set; }
    }
}
