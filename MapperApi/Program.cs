/***
 * Filename: Program.cs
 * Author : ebendutoit
 * Class   : Program
 *
 *      Configuration file for program startup
 ***/
using System;
using System.IO;
using System.Threading.Tasks;
using Mapper_Api.Context;
using Mapper_Api.Services;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mapper_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                    .UseConfiguration(
                            new ConfigurationBuilder()
                                    .SetBasePath(
                                            Directory.GetCurrentDirectory())
                                    // .AddJsonFile("hosting.json", true)
                                    .Build())
                    .UseStartup<Startup>()
                    .Build();
        }

    }
}
