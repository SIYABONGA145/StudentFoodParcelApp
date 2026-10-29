using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class Fundingstatus
{
    public int Fundingstatusid { get; set; }
    public string? Fundingstatus1 { get; set; }
    public int? Studentid { get; set; }
    public string? Reason { get; set; }

    // SQL: nvarchar
    public string? Updateddate { get; set; }

    public int? Financestaffid { get; set; }

    public virtual Financestaff? Financestaff { get; set; }
    public virtual Student? Student { get; set; }
}