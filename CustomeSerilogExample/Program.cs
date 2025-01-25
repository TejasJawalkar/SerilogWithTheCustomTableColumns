using CustomeSerilogExample.DependencyInjection;
using CustomeSerilogExample.Filters;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);


DependencyInjector.Injector(builder.Services);
#region Code To Save Logs Globally

#region OldCode
//builder.Host.ConfigureLogging((context, logging) =>
//{
//    // Clears default logging providers (to exclude ASP.NET Core generated logs)
//    logging.ClearProviders();

//    // Adding Serilog as the logging provider
//    logging.AddSerilog(new LoggerConfiguration()
//        .Enrich.FromLogContext()  // Enrich logs with the context (e.g., UserId, etc.)
//        .Enrich.WithProperty("ApplicationName", "MyApp")  // Static property for ApplicationName
//        .WriteTo.MSSqlServer(
//            connectionString: builder.Configuration.GetConnectionString("connectionstring"), // Connection string from appsettings.json or other config
//            tableName: "LogEvents",  // Table to store logs in SQL Server
//            autoCreateSqlTable: true, // Automatically create table if it doesn't exist
//            columnOptions: new Serilog.Sinks.MSSqlServer.ColumnOptions
//            {
//                AdditionalColumns = new List<Serilog.Sinks.MSSqlServer.SqlColumn>
//                {
//                    new Serilog.Sinks.MSSqlServer.SqlColumn { ColumnName = "UserId", DataType = System.Data.SqlDbType.NVarChar },
//                    new Serilog.Sinks.MSSqlServer.SqlColumn { ColumnName = "ControllerName", DataType = System.Data.SqlDbType.NVarChar},
//                    new Serilog.Sinks.MSSqlServer.SqlColumn { ColumnName = "MethodName", DataType = System.Data.SqlDbType.NVarChar},
//                    new Serilog.Sinks.MSSqlServer.SqlColumn { ColumnName = "MethodType", DataType = System.Data.SqlDbType.NVarChar},
//                    new Serilog.Sinks.MSSqlServer.SqlColumn { ColumnName = "AccessDateTime", DataType = System.Data.SqlDbType.DateTime },
//                }
//            })
//        .WriteTo.Console(outputTemplate: "{Message:lj} {Properties}{NewLine}") // Console log output for development
//        .Filter.ByExcluding(e => e.Properties.ContainsKey("SourceContext") &&
//            (e.Properties["SourceContext"].ToString().Contains("Microsoft") || e.Properties["SourceContext"].ToString().Contains("System"))) // Exclude logs from Microsoft and System namespaces
//        .Filter.ByExcluding(e => e.Properties.ContainsKey("RequestPath") &&  // Exclude static file requests like CSS, JS
//            (e.Properties["RequestPath"].ToString().Contains(".css") ||
//             e.Properties["RequestPath"].ToString().Contains(".js") ||
//             e.Properties["RequestPath"].ToString().Contains(".jpg") ||
//             e.Properties["RequestPath"].ToString().Contains(".png"))) // Exclude specific static file types
//        .MinimumLevel.Information()  // Set minimum log level to Information
//        .CreateLogger());
//});
#endregion

#region NewCode
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Read configuration from appsettings.json
    .Enrich.FromLogContext() //To Customize the Property of Logger {It adds new property like we have used UserId, ControllerName }
    .Filter.ByExcluding(logEvent =>
        logEvent.Properties.TryGetValue("SourceContext", out var sourceContext) &&
        (sourceContext.ToString().Contains("Microsoft") || sourceContext.ToString().Contains("System")))
    .Filter.ByExcluding(logEvent =>
        logEvent.Properties.TryGetValue("RequestPath", out var requestPath) &&
        (requestPath.ToString().Contains("browserLink") ||
         requestPath.ToString().Contains(".styles.css") ||
         requestPath.ToString().Contains("aspnetcore-browser-refresh.js")))
    .WriteTo.Console(LogEventLevel.Information) // Fallback console sink for debugging purposes {Its Optional If you Don't want to show on console just add in comment}
    .CreateLogger();

// Add Serilog to logging
builder.Host.UseSerilog();
#endregion

#endregion


// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddAuthentication();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddOptions();
builder.Services.AddControllersWithViews(option =>
{
    option.Filters.Add<LoggerFilter>();
});
builder.Services.AddOptions();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
