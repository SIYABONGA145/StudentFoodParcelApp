using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class Srcadmin
{
    public int Srcadminid { get; set; }

    public string? Termstart { get; set; }

    public string? Termend { get; set; }

    public int? Personid { get; set; }

    public int? Campusid { get; set; }

    public virtual ICollection<ApplicationStatus> ApplicationStatuses { get; set; } = new List<ApplicationStatus>();

    public virtual Campus? Campus { get; set; }

    public virtual ICollection<Distribution> Distributions { get; set; } = new List<Distribution>();

    public virtual ICollection<Foodparcelcycle> Foodparcelcycles { get; set; } = new List<Foodparcelcycle>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Person? Person { get; set; }
}