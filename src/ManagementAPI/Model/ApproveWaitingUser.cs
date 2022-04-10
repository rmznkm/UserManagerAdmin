using System;

namespace ManagementAPI.Model
{
    public class ApproveWaitingUser
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
