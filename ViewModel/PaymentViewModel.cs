namespace pet_spa_system1.ViewModel
{
    public class PaymentViewModel
    {
        public int PaymentId { get; set; }

        // Nếu không cần update các field này thì để nullable hoặc optional
        public int? OrderId { get; set; }

        public string? CustomerName { get; set; }

        public decimal? Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string? TransactionId { get; set; }

        public DateTime? PaymentDate { get; set; }
    }

}