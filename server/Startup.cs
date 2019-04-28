using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using server.Models;

namespace server
{
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<DoctorsContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

      services.AddSingleton<Services.Calendar>();

      services.AddSingleton<Services.BookedFiltrator>();


      services.AddMvc();

      services.AddCors();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseCors(builder => builder.WithOrigins("http://localhost:8080").AllowAnyHeader().AllowAnyMethod());
      }

      app.Use(async (context, next) =>
      {
        var path = context.Request.Path.Value;

        if (!path.StartsWith("/api") && !System.IO.Path.HasExtension(path))
        {
          context.Request.Path = "/index.html";
          await next();
        }
        else
        {
          await next();
        }
      });


      app.UseStaticFiles();

      app.UseMvc();
    }
  }
}
