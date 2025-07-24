using Microsoft.AspNetCore.Mvc;
using pet_spa_system1.Services;
using pet_spa_system1.Models;
using pet_spa_system1.ViewModel;
using System.Linq;

namespace pet_spa_system1.Controllers.ProductManager
{
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly PetDataShopContext _context;

        public PaymentController(IPaymentService paymentService, PetDataShopContext context)
        {
            _paymentService = paymentService;
            _context = context;
        }

        public IActionResult Payment()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetPayments()
        {
            var payments = _paymentService.GetAllPayments();
            return Json(payments);
        }

        [HttpGet]
        public IActionResult GetPayment(int id)
        {
            var payment = _paymentService.GetPaymentById(id);
            if (payment == null) return NotFound();
            // Map sang ViewModel nếu cần
            var vm = new PaymentViewModel
            {
                PaymentId = payment.PaymentId,
                OrderId = payment.OrderId,
                CustomerName = payment.User?.FullName ?? "",
                Amount = payment.Amount,
                PaymentMethod = payment.PaymentMethod?.MethodName ?? "",
                Status = payment.PaymentStatus?.StatusName ?? "",
                TransactionId = payment.TransactionId,
                PaymentDate = payment.PaymentDate
            };
            return Json(vm);
        }

        [HttpGet]
        public IActionResult GetPaymentMethods()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                Console.WriteLine("[AdminController] User not authenticated, redirecting or allowing anonymous access.");
                return RedirectToAction("AccessDenied", "Account");
            }
            // Lấy từ DB, ví dụ:
            var methods = _context.PaymentMethods.Select(m => new { m.PaymentMethodId, m.MethodName }).ToList();
            return Json(methods);
        }

        [HttpGet]
        public IActionResult GetPaymentStatuses()
        {
            var statuses = _context.PaymentStatuses.Select(s => new { s.PaymentStatusId, s.StatusName }).ToList();
            return Json(statuses);
        }

        [HttpPost]
        public IActionResult AddPayment([FromBody] PaymentViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var payment = new Payment
            {
                OrderId = model.OrderId,
                Amount = model.Amount,
                TransactionId = model.TransactionId,
                PaymentDate = model.PaymentDate,
                // Cần map PaymentMethodId, PaymentStatusId, UserId từ tên sang id
            };
            _paymentService.AddPayment(payment);
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdatePayment([FromBody] PaymentViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var payment = _paymentService.GetPaymentById(model.PaymentId);
            if (payment == null) return NotFound();
            payment.Amount = model.Amount;
            payment.TransactionId = model.TransactionId;
            payment.PaymentDate = model.PaymentDate;
            // Cập nhật các trường khác nếu cần
            _paymentService.UpdatePayment(payment);
            return Ok();
        }

        [HttpPost]
        public IActionResult DeletePayment(int id)
        {
            _paymentService.DeletePayment(id);
            return Ok();
        }
    }
}
