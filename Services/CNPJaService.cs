using System.Net.Http.Headers;
using pricewhisper.Models;

namespace pricewhisper.Services
{
    public class CNPJaService
    {
        private readonly HttpClient _httpClient;
        private const string API_KEY = "cae710ff-9e16-45d7-86f7-5ca5941da93d-68647162-57f9-4531-be42-98068541137d";
        private const string BASE_URL = "https://api.cnpja.com/";

        public CNPJaService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(API_KEY);
            _httpClient.BaseAddress = new Uri(BASE_URL);
        }

        public async Task<CNPJaResponse?> ConsultarCNPJ(string cnpj)
        {
            try
            {
                var response = await _httpClient.GetAsync($"office/{cnpj}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CNPJaResponse>();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
