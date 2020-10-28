using Domain.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class User
    {
        public User()
        {
            this.Transactions = new HashSet<Transaction>();
        }
        public long Id { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public TransactionType TransactionType { get; set; }
        public CommMedium CommunitactionType { get; set; }
        public ICollection<Transaction> Transactions { get; set; }

    }
}
