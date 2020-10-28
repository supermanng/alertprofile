using Application.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Entities
{
    public class User
    {
        public long Id { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public DateTime DateCreated { get; set; }
        public int RetryCount { get; set; }
        public SignUpChannel Channel { get; set; }
        public DateTime? LastLoginDate { get; set; }
    }
}
