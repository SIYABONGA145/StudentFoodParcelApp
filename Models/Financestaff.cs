using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class Financestaff
{
    public int Financestaffid { get; set; }

    public int? Personid { get; set; }

    public int? Campusid { get; set; }

    public int? Departmentid { get; set; }

    public virtual Campus? Campus { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Fundingstatus> Fundingstatuses { get; set; } = new List<Fundingstatus>();

    public virtual Person? Person { get; set; }
}