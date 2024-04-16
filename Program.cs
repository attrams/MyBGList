using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyBGList.Models;
using MyBGList.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
    {
        options.ParameterFilter<SortColumnFilter>();
        options.ParameterFilter<SortOrderFilter>();
    }
);
builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(cfg =>
            {
                cfg.WithOrigins(builder.Configuration["AllowedOrigins"]!);
                cfg.AllowAnyHeader();
                cfg.AllowAnyMethod();
            }
        );
        options.AddPolicy(name: "AnyOrigin", cfg =>
            {
                cfg.AllowAnyHeader();
                cfg.AllowAnyMethod();
                cfg.AllowAnyOrigin();
            }
        );
    }
);
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

if (app.Configuration.GetValue<bool>("useDeveloperExceptionPage"))
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};


app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapGet("/error", [EnableCors("AnyOrigin")][ResponseCache(NoStore = true)] () => Results.Problem());
app.MapGet("/error/test", [EnableCors("AnyOrigin")][ResponseCache(NoStore = true)] () => { throw new Exception("test"); });

app.MapGet("/cod/test", [EnableCors("AnyOrigin")][ResponseCache(NoStore = true)] () =>
    Results.Text(
        "<script>" +
        "window.alert('Your client supports JavaScript!" +
        "\\r\\n\\r\\n" +
        $"Server time (UTC): {DateTime.UtcNow.ToString("o")}" +
        "\\r\\n" +
        "Client time (UTC): ' + new Date().toISOString());" +
        "</script>" +
        "<noscript>Your client does not support JavaScript</noscript>",
        "text/html"
));

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
