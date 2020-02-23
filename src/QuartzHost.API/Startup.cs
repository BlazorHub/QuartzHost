using System.Data.SqlClient;
using System.Text.Encodings.Web;
using DG.Dapper;
using DG.Logger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QuartzHost.API.Common;
using QuartzHost.Core.Models;
using QuartzHost.Core.Services;

namespace QuartzHost.API
{
    public class Startup
    {
        private ILogger logger = DG.Logger.DGLogManager.GetLogger();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //����ע��DbContex
            var sqlConnString = Configuration.GetConnectionString("Mssql");
            services.AddSql(SqlClientFactory.Instance, sqlConnString);

            //����ע������
            var nodeSetting = Configuration.GetSection("NodeSetting").Get<NodeSetting>();
            nodeSetting.ConnStr = sqlConnString;
            services.AddSingleton(nodeSetting);

            //����ע��DGLogger
            services.AddLogging(x => x.AddDGLog());

            //�Զ�ע������ҵ��service
            services.AddAppServices();

            //json����
            services.AddControllers()
                .AddJsonOptions(option =>
                {
                    option.JsonSerializerOptions.PropertyNamingPolicy = null;
                    option.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });
            services.AddControllersWithViews(config =>
            {
                //���ȫ�ֹ�����
                config.Filters.Add(typeof(SimpleCheckAuthorization));
                config.Filters.Add(typeof(GlobalExceptionFilter));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        ///
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //��̬��Դ
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            var quartzService = app.ApplicationServices.GetService<IQuartzService>();
            quartzService.InitScheduler().Wait();
            quartzService.Start<Core.Common.TaskClearJob>("task-clear", "0 0/1 * * * ? *").Wait();
            lifetime.ApplicationStopping.Register(() =>
            {
                quartzService.Shutdown(true).Wait();
            });
        }
    }
}