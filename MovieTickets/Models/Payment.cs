using System;
using System.ComponentModel.DataAnnotations;

namespace MovieTickets.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Booking")]
        public int BookingId { get; set; }
        public Booking Booking { get; set; }

        [Required]
        [Display(Name = "Amount Paid")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public PaymentMethod Method { get; set; }

        [Required]
        [Display(Name = "Payment Status")]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [Display(Name = "Transaction ID")]
        public string TransactionId { get; set; } // from PayPal/Stripe/etc.

        [Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }

    // 🔹 Enum for Payment Method
    public enum PaymentMethod
    {
        CreditCard,
        DebitCard,
        PayPal,
        Cash,
        Wallet
    }

    // 🔹 Enum for Payment Status
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }
}
