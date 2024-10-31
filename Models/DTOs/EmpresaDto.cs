namespace pricewhisper.Models.DTOs
{
    public class EmpresaDto
    {
        public int EmpresaId { get; set; }
        public string CNPJ { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public List<UsuarioDto> Usuarios { get; set; }
    }
}
