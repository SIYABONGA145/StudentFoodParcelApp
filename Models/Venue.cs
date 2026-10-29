using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class Venue
{
    public int Venueid { get; set; }

    public string Venuename { get; set; } = null!;

    public int? Campusid { get; set; }

    public virtual Campus? Campus { get; set; }

    public virtual ICollection<Distribution> Distributions { get; set; } = new List<Distribution>();
}