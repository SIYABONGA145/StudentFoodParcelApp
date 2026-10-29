using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class Notification
{
    public int Notificationid { get; set; }
    public int? Studentid { get; set; }
    public int? Distributionid { get; set; }
    public string? Notificationtype { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }
    public string? Isread { get; set; }

    // SQL: datetime
    public DateTime? Createddate { get; set; }
    public DateTime? Readdate { get; set; }

    public int? Srcadminid { get; set; }

    public virtual Distribution? Distribution { get; set; }
    public virtual Srcadmin? Srcadmin { get; set; }
    public virtual Student? Student { get; set; }
}