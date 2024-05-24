using Abstraction.KeyValueStore;
using Infrastructure.KeyValueStore;
using Infrastructure.MessageBus;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddSingleton(typeof(IKeyValueStore<,>), typeof(KeyValueStoreInMemory<,>));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Core.IAssemblyMarker).Assembly)
);

builder.Services.AddMessageBus();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
