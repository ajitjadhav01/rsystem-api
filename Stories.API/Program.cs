using Stories.API.Contracts;
using Stories.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Adding HttpClient into services
builder.Services.AddHttpClient("Stories", httpClient => {
    httpClient.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
});


//Adding Caching into services
builder.Services.AddMemoryCache();

//Adding CROS
builder.Services.AddCors();

//Adding StoryServices in DI
builder.Services.AddTransient<IStoryService, StoryService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
