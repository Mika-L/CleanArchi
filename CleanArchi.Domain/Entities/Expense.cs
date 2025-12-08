using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchi.Domain.Entities
{
    public class Expense
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public Guid UserId { get; set; }
    }
}
