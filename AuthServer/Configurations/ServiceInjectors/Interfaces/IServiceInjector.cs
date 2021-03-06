﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Configurations.ServiceInjectors.Interfaces
{
	public interface IServiceInjector
	{
		void InjectServices(IConfiguration configuration, IServiceCollection services);
	}
}
