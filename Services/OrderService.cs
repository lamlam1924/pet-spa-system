using pet_spa_system1.Models;
using pet_spa_system1.Repositories;
using pet_spa_system1.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace pet_spa_system1.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;

        public OrderService(IOrderRepository orderRepository, ICartService cartService)
        {
            _orderRepository = orderRepository;
            _cartService = cartService;
        }
        public List<OrderViewModel> GetOrdersByUserId(int? userId)
        {
            if (userId == null) return new List<OrderViewModel>();
            var orders = _orderRepository.GetOrdersByUserId(userId.Value);
            return orders.Select(o => new OrderViewModel
            {
                OrderID = o.OrderId.ToString(),
                TotalAmount = o.TotalAmount,
                Status = o.Status != null ? o.Status.StatusName : (o.StatusId == 1 ? "Đang xử lý" : "Không xác định"),
                OrderDate = o.OrderDate,
                Items = o.OrderItems?.Select(oi => new OrderItemViewModel
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Không xác định",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new List<OrderItemViewModel>(),
                CustomerName = o.User?.FullName ?? "Không xác định",
                CustomerAddress = o.ShippingAddress ?? "Không xác định",
                CustomerPhone = o.User?.Phone ?? "Không xác định"
            }).ToList();
        }

        public OrderViewModel GetOrderDetail(int orderId)
        {
            var order = _orderRepository.GetOrderById(orderId);
            if (order == null) return null;

            return new OrderViewModel
            {
                OrderID = order.OrderId.ToString(),
                TotalAmount = order.TotalAmount,
                Status = order.Status != null ? order.Status.StatusName : "Không xác định",
                OrderDate = order.OrderDate,
                Items = order.OrderItems?.Select(oi => new OrderItemViewModel
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Không xác định",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new List<OrderItemViewModel>(),
                CustomerName = order.User?.FullName ?? "Không xác định",
                CustomerAddress = order.ShippingAddress ?? "Không xác định",
                CustomerPhone = order.User?.Phone ?? "Không xác định"
            };
        }

        public List<Order> GetOrdersByUserId(int userId)
          => _orderRepository.GetOrdersByUserId(userId);

        public Order GetOrderById(int id)
            => _orderRepository.GetOrderById(id);

        public void AddOrder(Order order)
            => _orderRepository.AddOrder(order);

        public void UpdateOrder(Order order)
            => _orderRepository.UpdateOrder(order);

        public void DeleteOrder(int id)
            => _orderRepository.DeleteOrder(id);

        public bool CancelOrder(int orderId, int? userId)
        {
            var order = _orderRepository.GetOrderById(orderId);
            if (order == null || order.UserId != userId) return false;
            if (order.StatusId == 3) return false; // Đã giao cho đơn vị vận chuyển

            order.StatusId = 5; // Đã hủy
            _orderRepository.UpdateOrder(order);

            // Thêm lại sản phẩm vào giỏ hàng
            foreach (var item in order.OrderItems)
            {
                _cartService.AddToCartAsync(userId.Value, item.ProductId, item.Quantity).Wait();
            }
            return true;
        }

        public List<OrderViewModel> GetOrdersByUserIdPaged(int? userId, int page, int pageSize, out int totalOrders)
        {
            totalOrders = 0;
            if (userId == null) return new List<OrderViewModel>();
            var orders = _orderRepository.GetOrdersByUserIdPaged(userId.Value, page, pageSize, out totalOrders);
            return orders.Select(o => new OrderViewModel
            {
                OrderID = o.OrderId.ToString(),
                TotalAmount = o.TotalAmount,
                Status = o.Status != null ? o.Status.StatusName : (o.StatusId == 1 ? "Đang xử lý" : "Không xác định"),
                OrderDate = o.OrderDate,
                Items = o.OrderItems?.Select(oi => new OrderItemViewModel
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Không xác định",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new List<OrderItemViewModel>(),
                CustomerName = o.User?.FullName ?? "Không xác định",
                CustomerAddress = o.ShippingAddress ?? "Không xác định",
                CustomerPhone = o.User?.Phone ?? "Không xác định",
                StatusId = o.StatusId
            }).ToList();
        }
    }
}
