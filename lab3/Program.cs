var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

var logFileName = $"log-{DateTime.Now:yyyyMMddHHmmss}.txt";
var logPath = $"Logs/{logFileName}";

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.Use(async (context, next) =>
{
    var request = context.Request;
    var start = DateTime.UtcNow;
    await next();
    var elapsed = DateTime.UtcNow - start;

    var fullUrl = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

    var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

    var remoteIpAddress = context.Connection.RemoteIpAddress;

    var logMessage = $"{fullUrl} | Time: {timestamp} | IP: {remoteIpAddress}{Environment.NewLine}";

    System.IO.File.AppendAllText(logPath, logMessage);

    //Console.WriteLine(logMessage);
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
