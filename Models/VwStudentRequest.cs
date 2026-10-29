using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class VwStudentRequest
{
    public int Requestid { get; set; }

    public int Studentid { get; set; }

    public string? RequestDate { get; set; }

    public string CycleApplyingFor { get; set; } = null!;

    public string StudentNumber { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string CampusName { get; set; } = null!;

    public int Campusid { get; set; }

    public string FundingStatus { get; set; } = null!;
}