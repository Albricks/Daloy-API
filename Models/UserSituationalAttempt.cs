using System;
using System.Collections.Generic;

namespace daloy_api.Models
{
    public class UserSituationalAttempt
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ActivityId { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public ICollection<UserSituationalAnswer> Answers { get; set; }
            = new List<UserSituationalAnswer>();
    }
}
