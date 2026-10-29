using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class Itadmin
{
    public int Itadminid { get; set; }

    public int? Personid { get; set; }

    public int? Campusid { get; set; }

    public virtual Campus? Campus { get; set; }

    public virtual Person? Person { get; set; }
}