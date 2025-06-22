using Microsoft.EntityFrameworkCore;
using SimpleDrive.Data;
using SimpleDrive.Middleware;
using SimpleDrive.Services;
using SimpleDrive.Storage;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<BlobService>();
builder.Services.AddDbContext<BlobDataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var storageType = builder.Configuration["Storage:Type"];

switch (storageType)
{
    case "S3":

        builder.Services.AddScoped<IStorageProvider, S3StorageProvider>();
        break;
    case "Database":

        builder.Services.AddScoped<IStorageProvider, DatabaseStorageProvider>();
        break;
    case "Local":

        builder.Services.AddScoped<IStorageProvider, LocalStorageProvider>();
        break;
    case "FTP":

        builder.Services.AddSingleton<IStorageProvider, FtpStorageProvider>();
        break;
    default:
        throw new Exception("Invalid storage type configured.");
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<SimpleAuthMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();
