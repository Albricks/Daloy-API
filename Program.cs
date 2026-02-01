using Azure.Storage.Blobs;
using daloy_api.Data;
using daloy_api.Models;
using daloy_api.Services;
using daloy_api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine(">>> APP STARTING");

// --------------------
// DATABASE (SAFE)
// --------------------
var dbConn = builder.Configuration.GetConnectionString("Default");

if (!string.IsNullOrEmpty(dbConn))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(dbConn));
}
else
{
    Console.WriteLine("⚠️ ConnectionStrings:Default is NOT configured.");
}

// --------------------
// IDENTITY
// --------------------
builder.Services
    .AddIdentity<AppUser, IdentityRole<Guid>>(options =>
    {
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

// --------------------
// JWT AUTHENTICATION
// --------------------
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "")
            )
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Method == HttpMethods.Options)
                {
                    context.NoResult();
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
        };
    });

// --------------------
// CORS (ANGULAR)
// --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200",
                "https://daloy.us",
                "https://www.daloy.us"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// --------------------
// AZURE BLOB STORAGE (SAFE — NO CRASH)
// --------------------
var blobConnString = builder.Configuration["AzureBlob:ConnectionString"];

if (!string.IsNullOrEmpty(blobConnString))
{
    builder.Services.AddSingleton(new BlobServiceClient(blobConnString));
    builder.Services.AddSingleton<BlobStorageService>();
}
else
{
    Console.WriteLine("⚠️ AzureBlob:ConnectionString NOT configured. Blob features disabled.");
}

// --------------------
// SERVICES
// --------------------
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAvatarService, AvatarService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddScoped<IVideoProgressService, VideoProgressService>();
builder.Services.AddScoped<IProgressService, ProgressService>();

// --------------------
// CONTROLLERS & SWAGGER
// --------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

Console.WriteLine(">>> APP BUILT");

// --------------------
// MIDDLEWARE (ORDER MATTERS)
// --------------------
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// ✅ CORS BEFORE AUTH
app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
