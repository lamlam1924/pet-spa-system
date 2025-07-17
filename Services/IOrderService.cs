using System.Collections.Generic;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Services
{
    public interface IOrderService
    {
        List<OrderViewModel> GetOrdersByUserId(int? userId);
        OrderViewModel GetOrderDetail(int orderId);
    }
}
