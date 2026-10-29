using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class Campus
{
    public int Campusid { get; set; }

    public string Campusname { get; set; } = null!;

    public int? Universityid { get; set; }

    public virtual ICollection<Financestaff> Financestaffs { get; set; } = new List<Financestaff>();

    public virtual ICollection<Itadmin> Itadmins { get; set; } = new List<Itadmin>();

    public virtual ICollection<Srcadmin> Srcadmins { get; set; } = new List<Srcadmin>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual University? University { get; set; }

    public virtual ICollection<Venue> Venues { get; set; } = new List<Venue>();
}