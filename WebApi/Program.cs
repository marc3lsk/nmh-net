using Abstraction.KeyValueStore;
using Core.Features.CMS.Persistence;
using Infrastructure.KeyValueStore;
using Infrastructure.MessageBus;
using Microsoft.EntityFrameworkCore;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);

// EF

builder.Services.AddDbContext<CmsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// App services

builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddSingleton(typeof(IKeyValueStore<,>), typeof(KeyValueStoreInMemory<,>));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Core.IAssemblyMarker).Assembly)
);

builder.Services.AddMessageBus();

// ASP.NET

builder.Services.AddControllers();

// Swagger

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ===== APP ====== //

var app = builder.Build();

// DB Migrate

using (var Scope = app.Services.CreateScope())
{
    var context = Scope.ServiceProvider.GetRequiredService<CmsDbContext>();
    await context.Database.MigrateAsync();
}

// Swagger

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ASP.NET

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
