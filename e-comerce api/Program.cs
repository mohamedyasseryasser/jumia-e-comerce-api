
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using e_comerce_api.services.reprosity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace e_comerce_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // builder.Services.AddSwaggerGen();
            // Swagger + JWT
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "My API",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "?? ?????? ??? ???????: Bearer {your token}"
                });

                c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            // Database
            builder.Services.AddDbContext<context>(option =>
                option.UseSqlServer(builder.Configuration.GetConnectionString("e-comerce"))
            );

            // Identity
            builder.Services.AddIdentity<applicationuser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<context>();

            // CORS
            builder.Services.AddCors(option =>
            {
                option.AddPolicy("employee", corspolicybuilder =>
                {
                    corspolicybuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });

            // Authentication JWT
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = "http://localhost:5064/",
                    ValidateAudience = true,
                    ValidAudience = "http://localhost:4200/",
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:secret"])
                    )
                };
            });

            // Repositories
            builder.Services.AddScoped<IAuth,Auth>();
            builder.Services.AddScoped<IUser, User>();
            builder.Services.AddScoped<ICategoryReprosity,Categoryreprosity>();
            builder.Services.AddScoped<Iimagesreprosity,Imagesreprosity>();
            builder.Services.AddScoped<IsubCategoryReprosity, SubCategoryReprosity>();
            builder.Services.AddScoped<Iproductreprosity, Productreprosity>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseStaticFiles();
            app.UseCors("employee");

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
