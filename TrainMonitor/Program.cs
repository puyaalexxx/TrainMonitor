using Microsoft.EntityFrameworkCore;
using TrainMonitor.DataBase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("MariaDB"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MariaDB"))
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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
