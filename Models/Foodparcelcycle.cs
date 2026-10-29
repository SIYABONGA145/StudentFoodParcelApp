using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class Foodparcelcycle
{
    public int Cycleid { get; set; }
    public string Cyclename { get; set; } = null!;

    // SQL: date
    public DateOnly? Startdate { get; set; }
    public DateOnly? Enddate { get; set; }

    public decimal? Budget { get; set; }
    public int? Maxbeneficiaries { get; set; }
    public string? Status { get; set; }
    public int? Srcadminid { get; set; }

    public virtual ICollection<Cycleitem> Cycleitems { get; set; } = new List<Cycleitem>();
    public virtual ICollection<Distribution> Distributions { get; set; } = new List<Distribution>();
    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
    public virtual Srcadmin? Srcadmin { get; set; }
}