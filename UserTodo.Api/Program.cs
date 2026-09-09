using Microsoft.EntityFrameworkCore;
using UserTodo.Api.Background;
using UserTodo.Api.Data;
using UserTodo.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

var dummyBaseUrl = builder.Configuration.GetValue<string>("DummyJson:BaseUrl");
builder.Services.AddHttpClient<DummyJsonClient>(client =>
{
    client.BaseAddress = new Uri(dummyBaseUrl);
});
builder.Services.AddScoped<SyncService>();
builder.Services.AddHostedService<SyncWorker>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
