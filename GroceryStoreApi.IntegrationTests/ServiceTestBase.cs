namespace GroceryStoreApi.IntegrationTests
{
    using GroceryStoreAPI;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using System;
    using System.IO;
    using System.Reflection;

    public abstract class ServiceTestBase
    {
        private Startup startup;

        /// <summary>Setups the environment.</summary>
        /// <param name="serviceCollecion">The service collection.</param>
        public void SetupEnvironment(Action<IServiceCollection> serviceCollecion = null)
        {
            var services = new ServiceCollection();
            var basePath = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Startup)).Location);

            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(m => m.ContentRootPath).Returns(basePath);
            envMock.Setup(m => m.EnvironmentName).Returns("Local");

#pragma warning disable CS0618 // Type or member is obsolete
            var hostingEnvMock = new Mock<IHostingEnvironment>();
            hostingEnvMock.Setup(m => m.ContentRootPath).Returns(basePath);
            hostingEnvMock.Setup(m => m.EnvironmentName).Returns("Local");

            services.AddSingleton<IHostingEnvironment>(hostingEnvMock.Object);
#pragma warning restore CS0618 // Type or member is obsolete

            startup = new Startup(envMock.Object)
            {
                ServiceCollection = serviceCollecion
            };

            startup.ConfigureServices(services);
            startup.ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>Gets the service provider.</summary>
        /// <value>The service provider.</value>
        public IServiceProvider ServiceProvider => startup?.ServiceProvider;
    }
}
