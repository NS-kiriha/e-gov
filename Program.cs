using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDistributedMemoryCache(); // Required for session
builder.Services.AddSession();

// Register Database Connection Service (injection)
builder.Services.AddSingleton<MySqlConnection>(serviceProvider =>
{
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
    return new MySqlConnection(connectionString);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
