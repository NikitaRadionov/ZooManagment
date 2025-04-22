using Presentation.Endpoints;
namespace ZooManagement.Server
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructure();
            builder.Services.AddApplication();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();


            app.MapGroup("/animals")
                .MapAnimalApi()
                .WithTags("Animals");

            app.MapGroup("/enclosures")
                .MapEnclosureApi()
                .WithTags("Enclosures");

            app.MapGroup("/feeding-schedules")
                .MapFeedingScheduleApi()
                .WithTags("Feeding Schedules");

            app.Run();
        }
    }
}
