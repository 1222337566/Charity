using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentCore.Domains.Jobs;
using InfrastructureManagmentCore.Domains.Messages;
using InfrastructureManagmentCore.Domains.NewFolder1;
using InfrastructureManagmentCore.Domains.Requests;
using InfrastructureManagmentDataAccess;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentDataAccess.Repositories;
using InfrastructureManagmentServices.Message;
using InfrastructureManagmentWebFramework.Helpers;
using InfrastructureManagmentWebFramework.Models;
using Microsoft.AspNetCore.Identity;
using InfrastructureManagmentServices;
using BillingWebApi.ActionFilters;
using InfrastructureManagmentServices.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.Services.AddDbContext<AppDbContext>(
               options => options.UseSqlServer("Data Source=DPP102;Initial Catalog=Infra;User Id=billingWebapi;Password=ax,%E`bt&N/F^nv#)f6k+; TrustServerCertificate=True;"));
builder.Services.AddCors();
builder.Services.AddControllers();
builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));
builder.Services.AddScoped<ISendService, SendService>();
builder.Services.AddTransient<IBaseRepository<BackgroundJob>, BaseRepository<BackgroundJob>>();
builder.Services.AddTransient<IBaseRepository<Balance>, BaseRepository<Balance>>();
builder.Services.AddTransient<IBaseRepository<Requests>, BaseRepository<Requests>>();
builder.Services.AddTransient<IBaseRepository<Error>, BaseRepository<Error>>();
builder.Services.AddTransient<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddTransient<IUnitofWork, UnitofWork>();
builder.Services.AddScoped<IAuthservice, AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IDataCollector<ReceiveModel>, DataCollector<ReceiveModel>>();
builder.Services.AddScoped<IDataCollector<HTSSendSMSModel>, DataCollector<HTSSendSMSModel>>();
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<RecordFilterAttribute>();
// Add  // configuration from appsettings.jsonb
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});

//var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();
//app.UseAuthentication();
app.MapControllers();

app.Run();
