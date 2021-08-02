using System.IO;
using System.Net;
using Amazon.S3;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using xCloud.Task7.Data;
using xCloud.Task7.Helpers;
using xCloud.Task7.Interfaces;
using xCloud.Task7.Services;

namespace xCloud.Task7
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AwsSettings"));
            
            var connectionString = Configuration.GetConnectionString("LocalPostgres");

            services.AddDbContextPool<BlobDbContext>(opt => 
                opt.UseNpgsql(connectionString));

            services.AddScoped<IImageService, ImageService>();

            services.AddTransient<IBucketService, BucketService>();
            services.AddTransient<IAwsService, AwsService>();
            
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(builder =>
                {
                    builder.Run(async httpContext =>
                    {
                        var feature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
                        httpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                        var exception = feature.Error;

                        string message = exception switch
                        {
                            AmazonS3Exception {ErrorCode: "InvalidAccessKeyId" or "InvalidSecurity"} => "Check the provided AWS Credentials",
                            AmazonS3Exception s3Exception => $"S3 bucket exception: {s3Exception.Message}",
                            IOException io => $"I/O exception: {io.Message}",
                            _ => "Oh, shit. I'm sorry"
                        };

                        await httpContext.Response.WriteAsync(message);
                    });
                });
                
                app.UseHsts();
            }
            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Images}/{action=UploadFile}/{id?}");
            });
        }
    }
}