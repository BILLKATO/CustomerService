using CustomerService;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var connection = builder.Configuration
    .GetConnectionString("Azure_connection_string");

builder.Services.AddDbContext<AdventureFiapContext>(options => options.UseSqlServer(connection));

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.MapGet("/Customers/{id}", (AdventureFiapContext context, int id) =>
{
    var customerAddress = context.CustomerAddresses
    .Where(ca => ca.CustomerId == id)
    .Select(ca => ca.AddressId)
    .ToList();

    var addressList = context.Addresses
    .Where(a => customerAddress
    .Contains(a.AddressId))
    .ToList();

    var customer = context.Customers
    .Where(c => c.CustomerId == id)
    .Single();

    return new
    {
        Customer = customer,
        Addresses = addressList
    };
})
.WithOpenApi();

app.Run();
