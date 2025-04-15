﻿using Microsoft.Data.SqlClient;

using System.ComponentModel.DataAnnotations.Schema;

namespace HealthCareApp.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public NotificationType notificationType { get; set; }
        public bool IsRead { get; set; }=false;
        public DateTime CreatedDate { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser? User { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
    public enum NotificationType
    {
        AppointmentReminder,
        Payment,
        Review,
        AppointmentReschedule
    }
}
