namespace PlayingWithCaching;

public static class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IServiceCollection services   = builder.Services;

        // Add services to the container
        {
            services.AddAuthorization();
        }

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline
        {
            app.UseAuthorization();

            app.MapGet("/", Gravatar.WriteGravatar);
        }

        app.UseAuthorization();

        app.Run();
    }
}