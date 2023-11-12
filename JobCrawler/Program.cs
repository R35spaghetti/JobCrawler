using JobCrawler.Repository;
using JobCrawler.Repository.Contract;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ArbetsformedlingenRepository>();
builder.Services.AddScoped<IndeedRepository>();


builder.Services.AddScoped<WebRepositoryResolver>(serviceProvider => key =>
{
    switch (key)
    {
        case "A":
            return serviceProvider.GetService<ArbetsformedlingenRepository>();
        case "B":
            return serviceProvider.GetService<IndeedRepository>();
        default:
            throw new KeyNotFoundException();
    }
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public delegate IWebRepository WebRepositoryResolver(string key);