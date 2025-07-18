using pet_spa_system1.Models;
using System.Linq;

namespace pet_spa_system1.Repositories
{
    public class OrderStatusRepository : IOrderStatusRepository
    {
        private readonly PetDataShopContext _context;

        public OrderStatusRepository(PetDataShopContext context)
        {
            _context = context;
        }

        public StatusOrder GetById(int id)
        {
            return _context.StatusOrders.FirstOrDefault(s => s.StatusId == id);
        }
    }
}
