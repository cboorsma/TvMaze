using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.Data;
using Model.Services;
using Model.Utilities;

namespace Model
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private string _connectionString;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _connectionString = "Server=tcp:************.database.windows.net,1433;Initial Catalog=tvmaze;Persist Security Info=False;User ID=***********;Password=*************;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            Console.WriteLine($"## Using MSSQL DB: {_connectionString}");
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(_connectionString)
            );

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped<ITvMazeService, TvMazeService>();
            services.AddMemoryCache();
            services.AddSingleton<IRequestService, RequestService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            app.UseMvc();
        }
    }
}