namespace spotify.core
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using services;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<FirestoreService>();
            services.AddSpotify();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) 
                app.UseDeveloperExceptionPage();
            app.Use((x, y) => // todo, temporary
            {
                x.Response.Headers["Access-Control-Allow-Origin"] = "*";
                return y();
            });
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
                endpoints.MapControllers());
        }
    }
}
