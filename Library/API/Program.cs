using Library.Data;
using Library.Data.Repositories;
using Library.Entities;
using Library.Interfaces;
using Library.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Porta
builder.WebHost.UseUrls("http://localhost:5000");

// Configuração para SQL Server (Requisito do Projeto)
// Tenta pegar do appsettings.json, senão usa um padrão local para desenvolvimento
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True;";

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddAutoMapper(typeof(Library.Mappings.MappingProfile));

builder.Services.AddScoped<IAutorRepository, AutorRepository>();
builder.Services.AddScoped<IAutorService, AutorService>();
builder.Services.AddScoped<ILivroRepository, LivroRepository>();
builder.Services.AddScoped<ILivroService, LivroService>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEmprestimoRepository, EmprestimoRepository>();
builder.Services.AddScoped<IEmprestimoService, EmprestimoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // Garante que o banco de dados seja criado com o schema mais recente.
    // Isso é útil em desenvolvimento para evitar o erro "no such table: __EFMigrationsHistory".
    db.Database.Migrate();


}

app.MapControllers();
app.Run();
