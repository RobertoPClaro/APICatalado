using APICatalogo.Context;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions
        .ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddCors(opciones =>
    opciones.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("https://apirequest.io")
        .AllowAnyMethod()
        .AllowAnyHeader();

    });
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var mappingConfig = new MapperConfiguration(mc =>
{                                                
    mc.AddProfile(new MappingProfile());        
});                                              
IMapper mapper = mappingConfig.CreateMapper();   
builder.Services.AddSingleton(mapper);         

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ApiCatalogo",
        Description = "Catálogo de Produtos e Categorias",
        TermsOfService = new Uri("https://roberto.net/terms"),
        Contact = new OpenApiContact
        {
            Name = "Roberto",
            Email = "robertopacheco1994@gmail.com",
            Url = new Uri("https://roberto.net"),
        },
        License = new OpenApiLicense
        {
            Name = "Usar sobre LICX",
            Url = new Uri("https://roberto.net/license"),
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() 
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Header de autorização JWT usando o esquema Bearer. \r\n\r\nInforme 'Bearer' [espaço] e token.\r\n\r\nExemplo: \'Bearer 1234abcdf\'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[]{}
        }
    });
});

string sqlConnection = builder.Configuration.GetConnectionString("DefaulConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(sqlConnection));

builder.Services.AddIdentity<IdentityUser, IdentityRole>() 
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT
// Adiciona o manipulador de autenticacao e define o
// esquema de autenticacao usando : Bearer
// valida o emissor , a audiencia e a chave
// usando a chave secreta valida a assinatura
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidAudience = builder.Configuration["TokenConfiguration:Audience"],
        ValidIssuer = builder.Configuration["TokenConfiguration:Issuer"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting(); 

app.UseAuthentication(); 

app.UseAuthorization();

app.UseCors(); 

app.MapControllers();

app.Run();
