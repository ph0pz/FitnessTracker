using FitnessTracker.API.DTOs;
using FitnessTracker.API.Interfaces;
using FitnessTracker.API.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace FitnessTracker.API.Services
{
    public class GptMacroService : IGptMacroService
    {
        private readonly FitnessTrackerDbContext _context;
        private readonly IConfiguration _config;
        private readonly HttpClient _httpClient;

        public GptMacroService(FitnessTrackerDbContext context , IConfiguration config)
        {
            _context = context;
            _config = config;
            _httpClient = new HttpClient();
        }

        public async Task<GptMacroResponseDto?> AnalyzeMacrosAsync(string promptText, int userId)
        {
            var apiKey = _config["OpenAI:ApiKey"];
            var model = _config["OpenAI:Model"];

            var prompt = $@"
        Estimate the macros for the following meal: ""{promptText}"".
        Respond only in JSON format like this:
        {{""Calories"": 400, ""Protein"": 30, ""Carbs"": 50, ""Fat"": 10}}";

            var requestBody = new
            {
                model = model,
                messages = new[] { new { role = "user", content = prompt } },
                temperature = 0.3
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            dynamic parsed = JsonConvert.DeserializeObject(json);
            string reply = parsed.choices[0].message.content;

            var macroDto = JsonConvert.DeserializeObject<GptMacroResponseDto>(reply);

            // Save to DB
            var suggestion = new GptmacroSuggestion
            {
                UserId = userId,
                InputText = promptText,
                OutputCalories = macroDto.Calories,
                OutputProtein = macroDto.Protein,
                OutputCarbs = macroDto.Carbs,
                OutputFat = macroDto.Fat,
                CreatedAt = DateTime.UtcNow
            };

            _context.GptmacroSuggestions.Add(suggestion);
            await _context.SaveChangesAsync();

            return macroDto;
        }
    
    }
}

