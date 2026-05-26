using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class OrderItemsDeliveryService : IOrderItemsDeliveryService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderItemsDeliveryService> _logger;

    public OrderItemsDeliveryService(
        HttpClient httpClient,
        ILogger<OrderItemsDeliveryService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task DeliverAsync(Order order)
    {
        try
        {
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
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Calling delivery function for order #{OrderId}", order.Id);

            var response = await _httpClient.PostAsync(string.Empty, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Successfully notified delivery function for order #{OrderId}", order.Id);
            }
            else
            {
                _logger.LogWarning("Delivery function returned status {StatusCode} for order #{OrderId}", 
                    response.StatusCode, order.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to call delivery function for order #{OrderId}", order.Id);
            // Don't throw - we don't want to block order creation if delivery notification fails
        }
    }
}
