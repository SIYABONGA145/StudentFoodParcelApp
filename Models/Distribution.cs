using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class Distribution
{
    public int Distributionid { get; set; }

    // SQL: date
    public DateOnly? Distributiondate { get; set; }

    public string? Distributiontime { get; set; }

    public int? Cycleid { get; set; }
    public int? Venueid { get; set; }
    public int? Srcadminid { get; set; }

    public virtual Foodparcelcycle? Cycle { get; set; }
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual Srcadmin? Srcadmin { get; set; }
    public virtual Venue? Venue { get; set; }
}