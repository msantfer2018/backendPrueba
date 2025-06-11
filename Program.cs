using ClientesApi.Data;
using ClientesApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// HABILITAR CORS PARA ANGULAR
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<ClientesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// USAR LA POLÍTICA CORS
app.UseCors("AllowAngular");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoint usando LINQ
app.MapGet("/api/clientes/linq", async (ClientesDbContext db, int page = 1, int pageSize = 10) =>
{
    var clientes = await db.Clientes
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return Results.Ok(clientes);
});

 //Endpoint usando Store Procedure
 app.MapGet("/api/clientes/sp", async (ClientesDbContext db, int page = 1, int pageSize = 10) =>
 {
     var clientes = await db.Clientes
         .FromSqlRaw("EXEC sp_GetClientesPaginado @Page = {0}, @PageSize = {1}", page, pageSize)
         .ToListAsync();

     return Results.Ok(clientes);
 });

app.Run();
