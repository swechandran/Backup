var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()  // Configure the app to run as a Windows service
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();  // Register the Worker service
    })
    .Build();

await host.RunAsync();
host.Run();


//install the service:
//    sc create StudentService binPath="C:\StudentService\StudentService.exe"

//Start the Service:
//    sc start StudentService

//stop the service:
//    sc stop StudentService

//sc stop StudentService:
//    sc delete StudentService








