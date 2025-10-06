using Microsoft.EntityFrameworkCore;
using TrainMonitor.Data.Repositories;
using TrainMonitor.DataBase;
using TrainMonitor.Extensions;
using TrainMonitor.Helpers;
using TrainMonitor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//DI
builder.Services.AddScoped<ITrainRepository, TrainRepository>();
builder.Services.AddScoped<ITrainService, TrainService>();

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionStringHost = EnvironmentUtils.IsRunningInContainer() ? "MariaDBContainer" : "MariaDB";
    var connectionString = builder.Configuration.GetConnectionString(connectionStringHost);

    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString)
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    await app.ApplyMigrationAsync(); // applying migrations automatically

    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

//app.UseAuthorization();

app.UseStaticFiles();
app.UseRouting();

app.MapDefaultControllerRoute();


await app.RunAsync();
