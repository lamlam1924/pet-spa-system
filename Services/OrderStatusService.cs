using pet_spa_system1.Repositories;

namespace pet_spa_system1.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly IOrderStatusRepository _orderStatusRepo;

        public OrderStatusService(IOrderStatusRepository orderStatusRepo)
        {
            _orderStatusRepo = orderStatusRepo;
        }

        public string GetStatusNameById(int id)
        {
            var status = _orderStatusRepo.GetById(id);
            return status?.StatusName ?? "Không xác định";
        }
    }
}
