using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Services;
using pet_spa_system1.ViewModel;

namespace pet_spa_system1.Controllers.OrderHistory
{
    [Route("OrderHistory")]
    public class OrderHistoryController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderHistoryController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("GetAllOrders")]
        public IActionResult GetAllOrders()
        {
            try
            {
                var orders = _orderService.GetAllOrders();
                return Json(orders);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi tải danh sách đơn hàng: " + ex.Message });
            }
        }

        [HttpGet("GetOrderDetail/{orderId}")]
        public IActionResult GetOrderDetail(int orderId)
        {
            try
            {
                var order = _orderService.GetOrderDetail(orderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }
                return Json(new { success = true, data = order });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi tải chi tiết đơn hàng: " + ex.Message });
            }
        }

        [HttpPost("UpdateStatus")]
        public IActionResult UpdateStatus([FromBody] UpdateOrderStatusModel model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var order = _orderService.GetOrderById(model.OrderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Kiểm tra trạng thái hiện tại
                if (order.StatusId == 4) // Đã giao
                {
                    return Json(new { success = false, message = "Không thể thay đổi trạng thái đơn hàng đã giao" });
                }

                if (order.StatusId == 5) // Đã hủy
                {
                    return Json(new { success = false, message = "Không thể thay đổi trạng thái đơn hàng đã hủy" });
                }

                // Cập nhật trạng thái
                order.StatusId = MapStatusNameToId(model.NewStatus);
                _orderService.UpdateOrder(order);

                return Json(new { success = true, message = "Cập nhật trạng thái đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi cập nhật trạng thái: " + ex.Message });
            }
        }

        [HttpPost("DeleteOrder")]
        public IActionResult DeleteOrder([FromBody] DeleteOrderModel model)
        {
            try
            {
                if (model == null)
                {
                    return Json(new { success = false, message = "Dữ liệu không hợp lệ" });
                }

                var order = _orderService.GetOrderById(model.OrderId);
                if (order == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
                }

                // Kiểm tra trạng thái đơn hàng trước khi xóa
                if (order.StatusId == 3 || order.StatusId == 4) // Đã giao vận chuyển hoặc Đã giao
                {
                    return Json(new { success = false, message = "Không thể xóa đơn hàng đã giao vận chuyển hoặc đã giao" });
                }

                _orderService.DeleteOrder(model.OrderId);
                return Json(new { success = true, message = "Xóa đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa đơn hàng: " + ex.Message });
            }
        }

        // Helper: Map status name to id
        private int MapStatusNameToId(string statusName)
        {
            switch (statusName)
            {
                case "Đang xử lí": return 1;
                case "Đang chuẩn bị": return 2;
                case "Đã giao vận chuyển": return 3;
                case "Đã giao": return 4;
                case "Đã hủy": return 5;
                default: return 1;
            }
        }

        // Helper: Map status id to name
        private string MapStatusIdToName(int statusId)
        {
            switch (statusId)
            {
                case 1: return "Đang xử lí";
                case 2: return "Đang chuẩn bị";
                case 3: return "Đã giao vận chuyển";
                case 4: return "Đã giao";
                case 5: return "Đã hủy";
                default: return "Đang xử lí";
            }
        }
    }

    public class UpdateOrderStatusModel
    {
        public int OrderId { get; set; }
        public string NewStatus { get; set; }
    }
    
    public class DeleteOrderModel
    {
        public int OrderId { get; set; }
    }
}