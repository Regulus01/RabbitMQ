using System.Data;
using System.Reflection;
using Consumer.AutoMapper;
using Consumer.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
                   "Database=postgres; " +
                   "User id=postgres; " +
                   "Password=example";

builder.Services.AddTransient<IDbConnection>(db => new NpgsqlConnection(dbConnection));

builder.Services.AddDbContext<MyDbContext>(opt =>
{
    opt.UseNpgsql(dbConnection, assembly =>
        assembly.MigrationsAssembly(typeof(MyDbContext).Assembly.FullName));
});

var connectionFactory = new ConnectionFactory
{
    HostName = "localhost",
    Port = 5672,
    UserName = "user",
    Password = "123qwe",
};

builder.Services.AddTransient<IConnectionFactory>(c => connectionFactory);

var mapper = AutoMapperConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
