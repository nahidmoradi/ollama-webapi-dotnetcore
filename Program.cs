using OllamaSharp;
using CommentAnalyzer.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register Ollama Client
builder.Services.AddSingleton<IOllamaApiClient>(sp =>
{
    var ollamaEndpoint = builder.Configuration["Ollama:Endpoint"] ?? "http://localhost:11434";
    var modelName = builder.Configuration["Ollama:ModelName"] ?? "llama3.1:latest";
    
    var client = new OllamaApiClient(ollamaEndpoint);
    client.SelectedModel = modelName;
    return client;
});

// Register our Comment Analyzer Service
builder.Services.AddScoped<ICommentAnalyzerService, CommentAnalyzerService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

Console.WriteLine(" Comment Analyzer API launched with Ollama!");
Console.WriteLine(" Swagger UI: http://localhost:5100/swagger");
Console.WriteLine(" Ollama Model: llama3.1:latest");

app.Run();
