using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using CombatLink.Infrastructure.Repositories;
using CombatLink.Domain.IRepositories;
using CombatLink.Domain.IServices;
using CombatLink.Application.Services;
using CombatLink.Web.Authentication;
using CombatLink.Infrastructure.BlobStorage;
using CombatLink.Web.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/LogIn"; 
        //options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;// extendva se vremeto ako si aktiven
    });
builder.Services.AddAuthorization();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IUserRepository>(provider =>
    new UserRepository(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserPreferenceRepository>(provider =>
    new UserPreferenceRepository(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISportRepository>(provider =>
    new SportsRepository(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ILanguageRepository>(provider =>
    new LanguageRepository(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ILikeRepository>(provider =>
    new LikeRepository(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMatchRepository>(provider =>
    new MatchRepository(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IChatMessageRepository>(provider =>
    new ChatMessageRepository(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISparringSessionProposalRepository>(provider =>
    new SparringSessionProposalRepository(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISportService, SportService>();
builder.Services.AddScoped<IPasswordHasher<Object>, PasswordHasher<Object>>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IUserPreferenceService, UserPreferenceService>();
builder.Services.AddScoped<IMatchmakingService, MatchmakingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddScoped<ILikeService, LikeService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IChatMessageService, ChatMessageService>();
builder.Services.AddScoped<ISparringSessionProposalService, SparringSessionProposalService>();

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapHub<ChatHub>("/chatHub");

app.Run();
