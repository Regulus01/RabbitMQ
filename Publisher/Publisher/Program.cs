using System.Data;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Publisher.Context;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


builder.Services.AddControllers();

var dbConnection = "Server=localhost:5432; " +
                        "Database=billingdb; " +
                        "User id=postgres; " +
                        "Password=example";

builder.Services.AddTransient<IDbConnection>(db => new NpgsqlConnection(dbConnection));

var connectionFactory = new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "user",
    Password = "123qwe",
};

builder.Services.AddTransient<IConnectionFactory>(c => connectionFactory);

builder.Services.AddDbContext<MyDbContext>(opt =>
{
    opt.UseNpgsql(dbConnection, assembly =>
        assembly.MigrationsAssembly(typeof(MyDbContext).Assembly.FullName));
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.UseHttpsRedirection();

app.Run();