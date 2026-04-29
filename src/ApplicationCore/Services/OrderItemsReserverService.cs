using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Azure.Messaging.ServiceBus;

using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

public class OrderItemsReserverService : IOrderItemsReserverService
{
    private readonly ServiceBusClient _client;
    private readonly string _queueName = "order-items-reservation";

    public OrderItemsReserverService(ServiceBusClient client)
    {
        _client = client;
    }

    public async Task ReserveAsync(Order order)
    {
        var sender = _client.CreateSender(_queueName);

        var request = new
        {
            orderId = order.Id.ToString(),
            buyerId = order.BuyerId,
            items = order.OrderItems.Select(i => new
            {
                itemId = i.ItemOrdered.CatalogItemId,
                quantity = i.Units
            })
        };

        var json = JsonSerializer.Serialize(request);

        var message = new ServiceBusMessage(json);

        await sender.SendMessageAsync(message);
    }
}
