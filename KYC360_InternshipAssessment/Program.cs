using Autofac;
using Autofac.Extensions.DependencyInjection;
using KYC360_InternshipAssessment.Controllers;
using KYC360_InternshipAssessment.Repository;
using KYC360_InternshipAssessment.Service;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;

namespace KYC360_InternshipAssessment
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());



            builder.Host.ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterType<EntitiesController>().SingleInstance();
                builder.RegisterType<EntityRepository>().As<IEntityRepository>().SingleInstance();
                builder.RegisterType<EntitiesService>().As<IEntityService>().SingleInstance();
            });

            builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())); ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(
            Directory.GetCurrentDirectory()),
                    RequestPath = "/CustomSwagger"
                });

                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/CustomSwagger/openapi.json", "MySwagger"));

            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}