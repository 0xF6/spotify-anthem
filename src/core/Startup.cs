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
            services.AddCors(x => 
                x.AddDefaultPolicy(q => q
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()));
            services.AddControllers()
                .AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) 
                app.UseDeveloperExceptionPage();
            app.UseCors();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
                endpoints.MapControllers());
        }
    }
}
