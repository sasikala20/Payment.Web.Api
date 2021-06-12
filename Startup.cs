using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.DatModel.Interface;
using Payment.Service;
using Payment.Service.Dto.Payment;
using Payment.Service.Infrastructure.DataContext;
using Payment.Service.Infrastructure.Repositories;
using DomainModel = Payment.DatModel.DomainModel.Transaction;
using ContextModel = Payment.Service.Infrastructure.DataContext.Model;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Payment.DatModel.DomainModel.Enumerations;
using Payment.Web.Api.Utilities;
namespace Payment.Web.Api
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
            .AddJsonOptions(options => {
              options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ssZ";
              options.SerializerSettings.Formatting = Formatting.Indented;
              options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
              options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
          });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen();
            
            SetDbConnetion(services);
            InjectDependencies(services);
            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseSwagger();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Payment.Web.Api");
                c.RoutePrefix = string.Empty;
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Payment}/{action=Get}/{id?}");
            });
        }

        private void SetDbConnetion(IServiceCollection services)
        {

            services.AddDbContext<PaymentContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                          sqlServerOptionsAction: sqlOptions =>
                          {
                              sqlOptions.EnableRetryOnFailure();
                          });
            });
        }
        private void InjectDependencies(IServiceCollection services)
        {
            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<int, Status>().ConvertUsing(s => (Status)s);
                cfg.CreateMap<Status,int >().ConvertUsing(s => (int)s);

                cfg.CreateMap<DomainModel.Payment, PaymentDto>()
                        .ForMember(x => x.Id, o => o.MapFrom(x => x.PaymentId))
                          .ForMember(x => x.CardHolderNumber, o => o.MapFrom(x => x.CardholderNumber.Mask(6,4,'*')));

                cfg.CreateMap<DomainModel.Payment, BasePaymentDto>()
                        .ForMember(x => x.Id, o => o.MapFrom(x => x.PaymentId));

                cfg.CreateMap<ContextModel.Payment, DomainModel.Payment>();               
                cfg.CreateMap<PaymentRequestDto, DomainModel.Payment>();


            });
            services.AddSingleton(sp => mapperConfiguration.CreateMapper());
            services.AddTransient<IPaymentService, PaymentService>();
            services.AddTransient<IPaymentRepository, PaymentRepository>();
        }
    }
}
