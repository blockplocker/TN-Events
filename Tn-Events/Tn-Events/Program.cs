using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tn_Events.Components;
using Tn_Events.Components.Account;
using DAL.Data;
using DAL.Models;
using DAL.Repositories;
using DAL.Repositories.Interfaces;
using Services.Services;
using Services.Services.Interfaces;

namespace Tn_Events
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.Stores.SchemaVersion = IdentitySchemaVersions.Version3;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

            // Repositories
            builder.Services.AddScoped<IEventRepository, EventRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IMemberRepository, MemberRepository>();

            // Services
            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IMemberService, MemberService>();

            var app = builder.Build();

            // Seed roles
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                string[] roles = ["Admin", "User"];

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            // mock data
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                if (!db.Categories.Any())
                {
                    var musik = new Category { Name = "Musik" };
                    var sport = new Category { Name = "Sport" };
                    var teknik = new Category { Name = "Teknik" };
                    var mat = new Category { Name = "Mat & Dryck" };
                    var konst = new Category { Name = "Konst" };

                    db.Categories.AddRange(musik, sport, teknik, mat, konst);
                    await db.SaveChangesAsync();

                    db.Events.AddRange(
                        new Event { Title = "Sommarkonsert i Parken", Description = "Livekonsert med lokala band under öppen himmel.", Address = "Stadsparken, Stockholm", StartDate = new DateTime(2026, 6, 15, 18, 0, 0), EndDate = new DateTime(2026, 6, 15, 22, 0, 0), Capacity = 200, IsCancelled = false, CategoryId = musik.Id },
                        new Event { Title = "Techno Night", Description = "Nattklubbsevent med internationella DJ:s.", Address = "Club Neon, Göteborg", StartDate = new DateTime(2026, 7, 5, 22, 0, 0), EndDate = new DateTime(2026, 7, 6, 4, 0, 0), Capacity = 150, IsCancelled = false, CategoryId = musik.Id },
                        new Event { Title = "Fotbollsturnering", Description = "5-mot-5 turnering för alla nivåer.", Address = "Zinkensdamms IP, Stockholm", StartDate = new DateTime(2026, 6, 20, 10, 0, 0), EndDate = new DateTime(2026, 6, 20, 17, 0, 0), Capacity = 40, IsCancelled = false, CategoryId = sport.Id },
                        new Event { Title = "Padel Open", Description = "Padelturning med priser för topp 3.", Address = "Padel Center, Malmö", StartDate = new DateTime(2026, 8, 10, 9, 0, 0), EndDate = new DateTime(2026, 8, 10, 16, 0, 0), Capacity = 32, IsCancelled = false, CategoryId = sport.Id },
                        new Event { Title = "AI & Framtiden", Description = "Seminariedag om artificiell intelligens och dess påverkan.", Address = "KTH, Stockholm", StartDate = new DateTime(2026, 9, 1, 9, 0, 0), EndDate = new DateTime(2026, 9, 1, 17, 0, 0), Capacity = 100, IsCancelled = false, CategoryId = teknik.Id },
                        new Event { Title = "Hackathon 2026", Description = "48-timmars kodutmaning med mentorer och priser.", Address = "Epicenter, Stockholm", StartDate = new DateTime(2026, 10, 15, 12, 0, 0), EndDate = new DateTime(2026, 10, 17, 12, 0, 0), Capacity = 60, IsCancelled = false, CategoryId = teknik.Id },
                        new Event { Title = "Street Food Festival", Description = "Maträtter från hela världen på ett och samma ställe.", Address = "Kungsträdgården, Stockholm", StartDate = new DateTime(2026, 7, 25, 11, 0, 0), EndDate = new DateTime(2026, 7, 25, 20, 0, 0), Capacity = 500, IsCancelled = false, CategoryId = mat.Id },
                        new Event { Title = "Vinprovning", Description = "Provning av ekologiska viner från Europa.", Address = "Winery Lounge, Göteborg", StartDate = new DateTime(2026, 8, 22, 18, 0, 0), EndDate = new DateTime(2026, 8, 22, 21, 0, 0), Capacity = 25, IsCancelled = false, CategoryId = mat.Id },
                        new Event { Title = "Konstutställning: Neon", Description = "Modern konst med neonljus som tema.", Address = "Moderna Museet, Stockholm", StartDate = new DateTime(2026, 6, 28, 10, 0, 0), EndDate = new DateTime(2026, 6, 28, 18, 0, 0), Capacity = 80, IsCancelled = false, CategoryId = konst.Id },
                        new Event { Title = "Keramik Workshop", Description = "Lär dig grunderna i keramik och ta hem ditt verk.", Address = "Konsthantverkshuset, Uppsala", StartDate = new DateTime(2026, 7, 12, 13, 0, 0), EndDate = new DateTime(2026, 7, 12, 16, 0, 0), Capacity = 15, IsCancelled = false, CategoryId = konst.Id },
                        new Event { Title = "Inställt Sportevent", Description = "Detta event har blivit inställt.", Address = "Arenan, Linköping", StartDate = new DateTime(2026, 9, 5, 14, 0, 0), EndDate = new DateTime(2026, 9, 5, 18, 0, 0), Capacity = 50, IsCancelled = true, CategoryId = sport.Id }
                    );
                    await db.SaveChangesAsync();
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.Run();
        }
    }
}
