using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Telegram.Bot;
using TgBotOrganaizer.Application;
using TgBotOrganaizer.Core.Entities;
using TgBotOrganaizer.InfrastructureMongo;

namespace TgBotOrganaizer
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

            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TgBotOrganaizer", Version = "v1" });
            });


            var authToken = this.Configuration["TgBot:AuthToken"];
            services.Configure<DotNetTheoryMongoStoreSettings>(Configuration.GetSection(nameof(DotNetTheoryMongoStoreSettings)));

            services.AddHttpClient("tgwebhook").AddTypedClient<ITelegramBotClient>(httpClient => new TelegramBotClient(authToken, httpClient));

            services.AddSingleton<IDotNetTheoryMongoStoreSettings>(sp => sp.GetRequiredService<IOptions<DotNetTheoryMongoStoreSettings>>().Value);

            this.ConfigureMongo(services);

            services.AddScoped<ITelegramMessageHandler, TelegramMessageHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TgBotOrganaizer v1"));
            }

            app.UseRouting();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureMongo(IServiceCollection serviceCollection)
        {
            MongoDbInitializer.Initialize();
            serviceCollection.AddScoped<IArticleRepository, ArticleMongoRepository>();
        }
    }
}
