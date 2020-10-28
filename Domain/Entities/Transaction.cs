using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string TransactionRefrenceNumber { get; set; }
        public TransactionType TransactionType { get; set; }
        public long UserId { get; set; }
        public User User { get; set; }
    }
}
