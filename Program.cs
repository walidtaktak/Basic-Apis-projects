using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ SERVICES
builder.Services.AddOpenApi();

// ✅ FIX CORS : Indispensable pour que React (5173) puisse parler à C# (5243)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ✅ HttpClient (On le garde prêt pour quand tu auras une nouvelle clé)
builder.Services.AddHttpClient("openai", client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    client.DefaultRequestHeaders.Authorization = 
        new AuthenticationHeaderValue("Bearer", "TON_TOKEN_ICI");
});

var app = builder.Build();

// 2️⃣ PIPELINE
app.MapOpenApi();
app.UseCors("AllowReactApp"); // Toujours avant les routes !

// ✅ On désactive la redirection HTTPS forcée en local pour éviter les soucis de certificats
// app.UseHttpsRedirection(); 

// ──────────────────────────────────────
// ROUTE CHATBOT :
// ──────────────────────────────────────

app.MapPost("/chatgpt", async (ChatRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.Input))
        return Results.BadRequest(new { reply = "Dis moi quelque chose, Simaw !" });

    // 🔬 SIMULATION JUMBO (Évite l'erreur 401 OpenAI)
    await Task.Delay(800); // Pour simuler le temps de réflexion de l'IA
    
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

// ──────────────────────────────────────
// ROUTE ATREEMO (PRÉPARATION)
// ──────────────────────────────────────

app.MapGet("/atreemo/customers", () => 
{
    // Simuler des données qui viendront de l'API Atreemo plus tard
    var mockCustomers = new[] {
        new { Id = 1, Name = "Simaw", Segment = "VIP" },
        new { Id = 2, Name = "Jumbo AI", Segment = "Tech" }
    };
    return Results.Ok(mockCustomers);
})
.WithName("GetAtreemoCustomers");

app.Run();

// ──────────────────────────────────────
// MODELS
// ──────────────────────────────────────

record ChatRequest(string Input);

static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
}