using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class Request
{
    public int Requestid { get; set; }

    // SQL: nvarchar
    public string? Requestdate { get; set; }

    public int? Studentid { get; set; }
    public int? Cycleid { get; set; }

    public virtual ICollection<ApplicationStatus> ApplicationStatuses { get; set; } = new List<ApplicationStatus>();
    public virtual Foodparcelcycle? Cycle { get; set; }
    public virtual Student? Student { get; set; }
}