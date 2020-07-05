using AutoMapper;
using AuthServer.Configurations.AutoMappings;
using AuthServer.Configurations.ServiceInjectors.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configurations.ServiceInjectors
{
    public class AutoMapperServicesInjector : IServiceInjector
    {
        public void InjectServices(IConfiguration configuration, IServiceCollection services)
        {
            MapperConfiguration config = new AutoMapper.MapperConfiguration(config => {
                config.AddProfile(new ModelToResponseProfile());
                config.AddProfile(new RequestToModelProfile());
            });
			
            IMapper mapper = config.CreateMapper();

            services.AddSingleton(mapper);
            services.AddTransient<ModelToResponseProfile>();
            services.AddTransient<RequestToModelProfile>();
        }
    }
}
