using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace pricewhisper.Models
{
    public class Empresa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmpresaId { get; set; }

        [Required]
        public string CNPJ { get; set; }

        [Required]
        public string RazaoSocial { get; set; }

        [Required]
        public string NomeFantasia { get; set; }

        public ICollection<Usuario>? Usuarios { get; set; }
    }
}
