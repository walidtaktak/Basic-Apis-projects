using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddHttpClient("openai", client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", "TON_TOKEN_ICI");
});

var app = builder.Build();

app.MapOpenApi();
app.UseCors("AllowReactApp");

app.MapPost("/chatgpt", async (ChatRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Input))
        return Results.BadRequest(new { reply = "Dis moi quelque chose, Simaw !" });

    await Task.Delay(800);
    
    string inputLower = request.Input.ToLower();
    string responseText;

    if (inputLower.Contains("atreemo"))
    {
        responseText = "En tant que Jumbo, je vois que tu veux parler d'Atreemo. C'est un CRM puissant ! On va bientôt brancher tes données clients ici.";
    }
    else if (inputLower.Contains("pfe"))
    {
        responseText = "Ton PFE C# / React avance bien ! L'architecture est validée. Prochaine étape : les données réelles.";
    }
    else 
    {
        responseText = $"Jumbo : J'ai bien reçu ton message '{request.Input}'. Je suis en mode simulation car ta clé OpenAI a besoin de crédits, mais ton code C# fonctionne à 100% !";
    }

    return Results.Ok(new { reply = responseText });
})
.WithName("ChatGpt");

app.MapGet("/atreemo/customers", () => 
{
    var mockCustomers = new[] {
        new { Id = 1, Name = "Simaw", Segment = "VIP" },
        new { Id = 2, Name = "Jumbo AI", Segment = "Tech" }
    };
    return Results.Ok(mockCustomers);
})
.WithName("GetAtreemoCustomers");

app.Run();

record ChatRequest(string Input);

static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}
