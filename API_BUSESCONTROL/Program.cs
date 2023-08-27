using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Helpers;
using API_BUSESCONTROL.Repository;
using API_BUSESCONTROL.Repository.Interfaces;
using API_BUSESCONTROL.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

namespace API_BUSESCONTROL
{
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            string connectionString = builder.Configuration.GetConnectionString("ConnectionMysql");
            builder.Services.AddDbContext<BancoContext>(options => options.UseMySql(connectionString, ServerVersion.Parse("8.1.32")));

            builder.Services.AddScoped<IOnibusRepository, OnibusRepository>();
            builder.Services.AddScoped<IPaletaCoresRepository, PaletaCoresRepository>();
            builder.Services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
            builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
            builder.Services.AddScoped<IContratoRepository, ContratoRepository>();
            builder.Services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            builder.Services.AddScoped<IFinanceiroRepository, FinanceiroRepository>();
            builder.Services.AddScoped<IRelatorioRepository, RelatorioRepository>();
            builder.Services.AddScoped<IUsuarioService, UsuarioService>();
            builder.Services.AddScoped<IEmail, Email>();
            builder.Services.AddScoped<ITokenService, TokenService>();


            var key = Encoding.ASCII.GetBytes(Settings.Secrect);
            builder.Services.AddAuthentication(auth => {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            .AddJwtBearer(x => {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            builder.Services.AddCors();

            // Permitir o ciclo de objetos � essencial para o funcionamento adequado do meu sistema, uma vez que a n�o permiss�o seria invi�vel para alguns servi�os.
            builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //Aceitando solicita��es http rest full de qualquer origem
            app.UseCors(options => {
                options.WithOrigins("http://localhost:4200") // Substitua pelo dom�nio do seu aplicativo
                       .AllowAnyMethod() // Permitir todos os m�todos HTTP (GET, POST, etc.)
                       .AllowAnyHeader() // Permitir todos os cabe�alhos
                       .AllowCredentials(); // Permitir envio de credenciais (por exemplo, cookies)
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}