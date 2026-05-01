using InfrastrfuctureManagmentCore.Persistence.Repositories.DirectoryUser;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Kanban;
using InfrastructureManagementRealtime.Helpers;
using InfrastructureManagementRealtime.Hubs;
using InfrastructureManagmentCore.Domains;
using InfrastructureManagmentCore.Domains.Identity;
using InfrastructureManagmentCore.Kanban;
using InfrastructureManagmentCore.Persistence.Repositories.Abstractions;
using InfrastructureManagmentCore.Persistence.Repositories.Ef;
using InfrastructureManagmentCore.Services.Calendar;
using InfrastructureManagmentCore.Services.Chat;
using InfrastructureManagmentCore.Services.Notifications;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentDataAccess.EntityFramework.EF;
using InfrastructureManagmentInfrastructure.Kanban;
using InfrastructureManagmentRealtime.Helpers;
using InfrastructureManagmentServices.Kanban;
using InfrastructureManagmentServices.Profile;
using InfrastructureManagmentServices.Search;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using Microsoft.AspNetCore.SignalR.StackExchangeRedis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using StackExchange.Redis;
using System.Text;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// CORS
var allowed = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(p => p.AddPolicy("client", x => x
    .WithOrigins(allowed).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

var key = builder.Configuration["Jwt:Key"]!;
var issuer = builder.Configuration["Jwt:Issuer"]!;
var audience = builder.Configuration["Jwt:Audience"]!;
var redisConnStr = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379,abortConnect=false";
var redisOptions = ConfigurationOptions.Parse(redisConnStr);
redisOptions.AbortOnConnectFail = false;   // نفس abortConnect=false
redisOptions.ConnectRetry = 5;
redisOptions.KeepAlive = 30;
redisOptions.ReconnectRetryPolicy = new ExponentialRetry(5000); // backoff

// Optional: TLS for Azure if not set in string
// redisOptions.Ssl = true;
// Repositories + UoW
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IChatRepository, ChatRepository>();
builder.Services.AddScoped<ICalendarRepository, CalendarRepository>();
builder.Services.AddScoped<ILdapDirectoryService, LdapDirectoryService>();
builder.Services.AddScoped<IAdImportService, AdImportService>();
builder.Services.AddScoped<IDirectoryUserRepository, DirectoryUserRepository>();
builder.Services.AddScoped<IAdPromoteService, AdPromoteService>();
builder.Services.AddScoped<IAdDirectorySyncService, AdDirectorySyncService>();
// Services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<IDirectoryDeviceRepository, DirectoryDeviceRepository>();
builder.Services.AddScoped<INetworkDeviceService, NetworkDeviceService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ICalendarService, CalendarService>();
// Create multiplexer singleton
builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
    ConnectionMultiplexer.Connect(redisOptions));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true
        };
        o.RequireHttpsMetadata = false;
        o.SaveToken = true;
        // السماح بتمرير التوكن كـ access_token في كويري الهاندشيك
        o.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var token = ctx.Request.Query["access_token"];
                var path = ctx.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(token) && (path.StartsWithSegments("/hubs/notify") || path.StartsWithSegments("/hubs/chat") || path.StartsWithSegments("/hubs/kanban")))
                    ctx.Token = token;
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddHostedService<InfrastructureManagementRealtime.Services.CalendarReminderService>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BillingWebAPIDbconnection")));
builder.Services.AddSingleton<IRoomPresencecs, RedisRoomPresence>();
builder.Services.AddAuthorization();
builder.Services.AddSignalR(o => o.EnableDetailedErrors = true)
    .AddStackExchangeRedis(builder.Configuration.GetConnectionString("Redis"), options =>
    {
        options.Configuration.ChannelPrefix = "infra-realtime";
    });
builder.Services.AddSingleton<INotificationBus, NotificationBus>();



builder.Services.AddControllers();
builder.Services.AddSingleton<IUserIdProvider, NameIdUserIdProvider>();
builder.Services.AddScoped<ITaskBoardRepository, TaskBoardRepository>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IKanbanService, KanbanService>();
builder.Services.AddScoped<IKanbanRepository, KanbanRepository>();
builder.Services.AddScoped<IBoardAccessRepository, BoardAccessRepository>();
builder.Services.AddScoped<IAdReader, LdapAdReader>();
builder.Services.AddScoped<IDeviceRepo, DeviceRepo>();
builder.Services.AddScoped<IDhcpReader, DhcpWindowsReader>();

// (اختياري) Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddIdentityCore<ApplicationUser>(opt => { /* خياراتك */ })
    .AddRoles<IdentityRole>()   // اختياري
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
var app = builder.Build();

app.UseCors("client");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationsHub>("/hubs/notify1");
//app.UseCors("AllowSkote");
// app.UseAuthentication();
// app.UseAuthorization();
app.MapHub<NotifyHub>("/hubs/notify");
app.MapHub<KanbanHub>("/hubs/kanban");
app.MapGet("/auth/whoami", (HttpContext ctx) => new {
    name = ctx.User?.Identity?.Name,
    id = ctx.User?.FindFirst("sub")?.Value
        ?? ctx.User?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value
}).RequireAuthorization();

//app.MapPost("/notify/user/by-user", async (
//    string username,
//    string title,
//    string message,
//    UserManager<ApplicationUser> userManager,
//    INotificationBus bus) =>
//{
//    var user = await userManager.FindByNameAsync(username);
//    if (user is null) return Results.NotFound(new { error = "User not found" });

//    var payload = new NotificationPayload(title, message, null, null, "info", DateTime.UtcNow.Ticks);
//    await bus.ToUserAsync(user.Id, payload);
//    return Results.Ok(new { ok = true, userId = user.Id });
//}).RequireAuthorization();
// (اختياري) Endpoint اختبار — يتطلب Auth بنفس JWT/Identity


//app.MapPost("/notify/test", async (INotificationBus bus, HttpContext ctx) =>
//{
//    var userId = ctx.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
//    if (string.IsNullOrEmpty(userId)) return Results.Unauthorized();

//    var payload = new NotificationPayload(
//        title: "Test notification",
//        message: "Hello from /notify/test",
//        url: null,
//        icon: null,
//        level: "info",
//        atUtcTicks: DateTime.UtcNow.Ticks
//    );

//    await bus.ToUserAsync(userId, payload);
//    return Results.Ok(new { ok = true });
//}).RequireAuthorization();

app.MapHub<ChatHub>("/hubs/chat");
// (اختياري) Swagger + Health
app.UseSwagger(); app.UseSwaggerUI();
app.MapGet("/healthz", () => Results.Ok(new { ok = true, ts = DateTime.UtcNow }));

app.Run();
