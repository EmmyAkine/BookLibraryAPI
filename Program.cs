using BookLibraryAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Text;

namespace BookLibraryAPI {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            //DbContext registration
            builder.Services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            //Identity registration
            builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<LibraryDbContext>();

            //Controller registration
            builder.Services.AddControllers();

            //Authentication & Authorization registration
            builder.Services.AddAuthentication( options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });
            builder.Services.AddAuthorization();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi(options => {
                options.AddDocumentTransformer((document, context, cancellationToken) => {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                    document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter your JWT token to authorize."
                    };

                    document.Security ??= new List<OpenApiSecurityRequirement>();
                    document.Security.Add(new OpenApiSecurityRequirement {
                        [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                    });
                    return Task.CompletedTask;
                });
            });


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c => {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: {your token}"
                });
                c.AddSecurityRequirement((doc) => new OpenApiSecurityRequirement {
                    [new OpenApiSecuritySchemeReference("Bearer", doc)] = []
                });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment()) {
                app.MapOpenApi();
                app.MapScalarApiReference(); //Scalar

                //Swagger

                app.UseSwagger();
                /*app.UseSwaggerUI(options => {
                    options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
                });*/
                app.UseSwaggerUI();
            }

            //app.usehtt

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            using (var scope = app.Services.CreateScope()) {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await SeedRoles(roleManager);
            }
            app.Run();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager) {
            if (!await roleManager.RoleExistsAsync("Admin")) {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            if (!await roleManager.RoleExistsAsync("User")) {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }

    }

}
