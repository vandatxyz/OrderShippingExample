using MassTransit;
using SharedMessages.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        cfg.Message<OrderPlaced>( x => x.SetEntityName("order-placed-exchange"));
        cfg.Publish<OrderPlaced>( p => p.ExchangeType = "direct");
        //cfg.Publish<OrderPlaced>( p => p.ExchangeType = "fanout");
        // Additional configuration can be added here if needed
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapPost("/orders", async (OrderRequest orderRequest, IBus bus) =>
{
    var orderPlaceMessage = new OrderPlaced(orderRequest.OrderId, orderRequest.Quantity);
    await bus.Publish(orderPlaceMessage, context =>
    {
        context.SetRoutingKey(orderPlaceMessage.Quantity > 10 ? "order.shipping" : "order.tracking");
    });

    return Results.Created($"/orders/{orderRequest.OrderId}", orderPlaceMessage);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();

public record OrderRequest(Guid OrderId, int Quantity);