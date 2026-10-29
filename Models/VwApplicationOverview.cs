using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class VwApplicationOverview
{
    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string Studentnumber { get; set; } = null!;

    public string? Requestdate { get; set; }

    public string Cyclename { get; set; } = null!;

    public string? Startdate { get; set; }

    public string? Enddate { get; set; }

    public string? Applicationstatus { get; set; }

    public string? DecisionReason { get; set; }

    public string? DecisionDate { get; set; }
}