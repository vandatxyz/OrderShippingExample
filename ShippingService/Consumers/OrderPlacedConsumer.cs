using MassTransit;
using SharedMessages.Messages;

namespace ShippingService.Consumers
{
    public class OrderPlacedConsumer : IConsumer<OrderPlaced>
    {
        public Task Consume(ConsumeContext<OrderPlaced> context)
        {
            Console.WriteLine($"OrderPlaced received: OrderId={context.Message.OrderId}, Quantity={context.Message.Quantity}");
            return Task.CompletedTask;
        }
    }
}
