namespace pricewhisper.Models
{
    public class CNPJaResponse
    {
        public string? TaxId { get; set; }
        public bool? Active { get; set; }
        public CNPJaCompany? Company { get; set; }
    }

    public class CNPJaCompany
    {
        public string? Name { get; set; }
        public CNPJaStatus? Status { get; set; }
    }

    public class CNPJaStatus
    {
        public int Id { get; set; }
        public string? Text { get; set; }
    }
}
