using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class Department
{
    public int Departmentid { get; set; }

    public string Departmentname { get; set; } = null!;

    public int? Universityid { get; set; }

    public virtual ICollection<Financestaff> Financestaffs { get; set; } = new List<Financestaff>();

    public virtual University? University { get; set; }
}