using financial_analytics_infrastructure.Db;
using FinancialAnalytics.Application;
using FinancialAnalytics.Core;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var entityTypes = typeof(IEntity).Assembly
                                        .GetTypes()
                                        .Where(x => !x.IsInterface &&
                                                    !x.IsAbstract &&
                                                    typeof(IEntity).IsAssignableFrom(x));


var serviceTypes = typeof(IServise).Assembly.GetTypes().Where(x => !x.IsInterface &&
                                                    !x.IsAbstract &&
                                                    typeof(IEntity).IsAssignableFrom(x));

foreach (var entityType in entityTypes)
    builder.Services
        .AddSingleton(typeof(BasicRepository<>).MakeGenericType(entityType),
                      typeof(BasicRepository<>).MakeGenericType(entityType));

foreach (var serviceType in serviceTypes)
    builder.Services.AddSingleton(serviceType);

builder.Services.AddCors(op =>
{
    op.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin() 
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting(); 

app.UseCors();
app.UseHttpsRedirection();

app.Run();
