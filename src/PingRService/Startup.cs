// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The startup class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PingRService;

/// <summary>
/// The startup class.
/// </summary>
public class Startup
{
    /// <summary>
    /// The PingR service configuration.
    /// </summary>
    private readonly PingRConfiguration pingRServiceConfiguration = new();

    /// <summary>
    /// Initializes a new instance of <see cref="Startup"/> class.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public Startup(IConfiguration configuration)
    {
        configuration.GetSection(Program.ServiceName).Bind(this.pingRServiceConfiguration);
    }

    /// <summary>
    /// Configures the services.
    /// </summary>
    /// <param name="services">The services.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        // Add the configuration.
        services.AddOptions();
        services.AddSingleton(this.pingRServiceConfiguration);

        // Add the logger.
        services.AddSingleton(Log.Logger);

        // Add JSON converters.
        services.AddControllers()
            .AddJsonOptions(
                o =>
                {
                }).AddRazorPagesOptions(options => options.RootDirectory = "/")
            .AddDataAnnotationsLocalization();

        services.AddSingleton(p => new PingRService(this.pingRServiceConfiguration));
        services.AddSingleton<IHostedService>(p => p.GetRequiredService<PingRService>());

        services.AddConnections();
    }

    /// <summary>
    /// This method gets called by the runtime.
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="env">The web hosting environment.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSerilogRequestLogging();
        app.UseRouting();

        app.UseEndpoints(endpoints => 
        {
            endpoints.MapControllers();
        });
    }
}
