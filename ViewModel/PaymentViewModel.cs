namespace pet_spa_system1.ViewModel
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string Status { get; set; }
        public string TransactionId { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}