using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class ApplicationStatus
{
    public int Applicationstatusid { get; set; }
    public string? Applicationstatus { get; set; }
    public string? DecisionReason { get; set; }

    // SQL: datetime
    public DateTime? DecisionDate { get; set; }

    public int? Requestid { get; set; }
    public int? Srcadminid { get; set; }

    public virtual Request? Request { get; set; }
    public virtual Srcadmin? Srcadmin { get; set; }
}