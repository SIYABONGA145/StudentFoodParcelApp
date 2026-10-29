using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class University
{
    public int Universityid { get; set; }

    public string Universityname { get; set; } = null!;

    public virtual ICollection<Campus> Campuses { get; set; } = new List<Campus>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}