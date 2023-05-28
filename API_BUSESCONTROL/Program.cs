using API_BUSESCONTROL.Data;
using API_BUSESCONTROL.Helpers;
using API_BUSESCONTROL.Repository;
using Microsoft.EntityFrameworkCore;

namespace API_BUSESCONTROL {
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
            builder.Services.AddScoped<IEmail, Email>();

            builder.Services.AddCors();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            //Aceitando solicitações http rest full de qualquer origem
            app.UseCors(opcoes => opcoes.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.MapControllers();

            app.Run();
        }
    }
}