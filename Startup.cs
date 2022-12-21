using CoNaObiad.Authorization;
using CoNaObiad.Entity;
using CoNaObiad.Middleware;
using CoNaObiad.Models;
using CoNaObiad.Models.Validators;
using CoNaObiad.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoNaObiad
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var aunthenticationSettings = new AuthenticationSettings();

            Configuration.GetSection("Authentication").Bind(aunthenticationSettings);

            services.AddSingleton(aunthenticationSettings);
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = "Bearer";
                option.DefaultScheme = "Bearer";
                option.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false; //nie wymuszamy tylko przez protokol https
                cfg.SaveToken = true; //przekazujemy informacje ze dany token powinien zostac zzapisany z serwera do autentykacji
                cfg.TokenValidationParameters = new TokenValidationParameters // parametry walidacji zeby sprawdzic czy dany token od klienta jest zgodny z serwrowym
                {
                    ValidIssuer = aunthenticationSettings.JwtIssuer, //wydawca danego tokenu
                    ValidAudience = aunthenticationSettings.JwtIssuer, //kto moze uzywac tego tokenu
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(aunthenticationSettings.JwtKey)), //klucz prywatny wygenerowany na podstawie jwtkey w authenticsettings.json
                };
            });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasNationality", builder => builder.RequireClaim("Natinality"));
                options.AddPolicy("Atleast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
            }); //w³asny warunek autoryzacji

            services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
            services.AddControllers().AddFluentValidation();

            services.AddScoped<CoNaObiadSeeder>();
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddScoped<IDishService, DishService>();
            services.AddScoped<IIngredientService, IngredientService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ErrorHandlingMiddleware>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
            services.AddScoped<IValidator<DishQuery>, DishQueryValidator>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen();
            services.AddCors(options =>
            {
                options.AddPolicy("FrontEndClient", builder =>

                builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithOrigins(Configuration["AllowedOrigins"])

                    );
            });

            services.AddDbContext<DishDbContext>
                (options => options.UseSqlServer(Configuration.GetConnectionString("CoNaObiadDbConnection")));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, CoNaObiadSeeder seeder)
        {
            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseCors("FrontEndClient");
            seeder.Seed();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseAuthentication();
            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Co Na Obiad");

            });

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
