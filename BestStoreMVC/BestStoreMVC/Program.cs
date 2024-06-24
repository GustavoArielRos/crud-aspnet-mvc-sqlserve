using BestStoreMVC.Services;
using Microsoft.EntityFrameworkCore;

namespace BestStoreMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //configura o contexto do banco  de dados adicionando o Entidade Framework Core
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {   
                //"DefaultConnection" é o nome da conexão que demos no appsetting.json da conectionstring com o banco de dados
                var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");//obtém a string de conexão
                options.UseSqlServer(connectionString);//Configura o contexto para usar o sql com a string de conexão
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
