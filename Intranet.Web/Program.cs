using Intranet.Constants;
using Intranet.Core.Interfaces.Repositories;
using Intranet.Core.Interfaces.Services;
using Intranet.Core.Services;
using Intranet.Helpers;
using Intranet.Hubs;
using Intranet.Infrastructure.Contexts;
using Intranet.Infrastructure.Email;
using Intranet.Infrastructure.Identity;
using Intranet.Infrastructure.Repositories;
using Intranet.Infrastructure.Services;
using Intranet.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

#region AddSwaggerGen
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Intranet APIs",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer <Token>\"",
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", document)] = []
    });
});
#endregion

#region jwtauthentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
      .AddJwtBearer(options =>
      {
          options.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuer = true,
              ValidateAudience = true,
              ValidateLifetime = true,
              ValidateIssuerSigningKey = true,
              ValidIssuer = builder.Configuration.GetSection("Jwt").GetValue<string>("ValidIssuer"),
              ValidAudience = builder.Configuration.GetSection("Jwt").GetValue<string>("ValidAudience"),
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt").GetValue<string>("SecretKey")))
          };
          options.Events = new JwtBearerEvents
          {
              OnTokenValidated = async context =>
              {

                  var userIdClaim = context.Principal.FindFirst(ClaimTypes.NameIdentifier)
                      ?? context.Principal.FindFirst(JwtRegisteredClaimNames.Sub);

                  if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var employeeId))
                  {
                      context.Fail("Invalid employee ID");
                      return;
                  }
                  var dbContext = context.HttpContext.RequestServices.GetRequiredService<IntranetDBContext>();
                  var employee = await dbContext.Employees
                    .AsNoTracking()
                    .Where(x => x.Id == employeeId)
                    .Select(x => new { x.Id, x.Role, x.LoginId })
                    .FirstOrDefaultAsync();

                  if (employee == null || string.IsNullOrWhiteSpace(employee.LoginId))
                  {
                      context.Fail("Employee does not exist or is inactive");
                      return;
                  }

                  var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                  var existingRoles = claimsIdentity.FindAll(ClaimTypes.Role).ToList();
                  foreach (var claim in existingRoles)
                  {
                      claimsIdentity.RemoveClaim(claim);
                  }

                  // Refresh role from Employee (same scalar Role column used by WebForms)
                  claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, employee.Role ?? string.Empty));
              },
              //REQUIRED for SignalR JWT over WebSockets
              OnMessageReceived = context =>
              {

                  if (context.HttpContext.Request.Path.StartsWithSegments("/signalRHub"))
                  {
                      var accessToken = context.Request.Query["access_token"];
                      if (!string.IsNullOrEmpty(accessToken))
                      {
                          context.Token = accessToken;
                      }
                  }

                  return Task.CompletedTask;
              }
          };

          //options.Events = new JwtBearerEvents
          //{
          //    OnMessageReceived = context =>
          //    {

          //        if (context.HttpContext.Request.Path.StartsWithSegments("/signalRHub"))
          //        {
          //            var accessToken = context.Request.Query["access_token"];
          //            if (!string.IsNullOrEmpty(accessToken))
          //            {
          //                context.Token = accessToken;
          //            }
          //        }

          //        return Task.CompletedTask;
          //    }
          //};
      });
#endregion

#region Registering DI Services
// Configuring Services
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IAuthUser, AuthUser>();
builder.Services.AddScoped<IEmailService, GmailSmtpEmailService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILdapAuthService, LdapAuthService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<IActivityService, ActivityService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<INotificationHubService, NotificationHubService>();

// Configuring Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
#endregion

#region Configure Controllers with Newtonsoft.Json and Reference Loop Handling
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });
#endregion

#region Allow users to upload files up to 100 MB
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600;
});
#endregion

// AutoMapper Profiles should be AddSingleton
#region Register AutoMapper Profiles 
builder.Services.RegisterProfiles();
#endregion

#region AddHttpClient
builder.Services.AddHttpClient();
#endregion

#region AddDbContext
builder.Services.AddDbContextFactory<IntranetDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IntranetDBConnection"));

}, ServiceLifetime.Scoped);
#endregion

#region Enable CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200"
            )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials() // REQUIRED for SignalR
              .WithExposedHeaders(Headers.PaginationHeaderKey);
    });
});
#endregion


#region Add SignalR Service
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = false;
    hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(15);    // Ping every 15 seconds
    hubOptions.ClientTimeoutInterval = TimeSpan.FromSeconds(30); // Wait 30 seconds for client response
    hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(60);
});
#endregion

#region Add Health Checks
builder.Services.AddHealthChecks();
#endregion



var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAngular");   // MUST be after routing
app.UseAuthentication();
app.UseAuthorization();

app.UseWebSockets(new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(15)  // Default is 2
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

app.MapControllers();
app.MapHub<SignalRHub>("/signalRHub");

// Health Check (after UseRouting, before endpoint mappings)
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = new
        {
            status = report.Status.ToString(),
            results = report.Entries.Select(e => new
            {
                key = e.Key,
                status = e.Value.Status.ToString(),
                error = e.Value.Exception?.Message,
                duration = e.Value.Duration
            })
        };
        await context.Response.WriteAsJsonAsync(result);
    }

});


app.Run();
