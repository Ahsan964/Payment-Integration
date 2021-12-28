using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication3.Models
{
    public class PaymentVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Email { get; set; }
        public string StripeToken { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Phone { get; set; }
    }
}