using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class VwStudentProfile
{
    public int Personid { get; set; }

    public int Studentid { get; set; }

    public int Campusid { get; set; }

    public int Universityid { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string? Emailaddress { get; set; }

    public string? Phonenumber { get; set; }

    public string? Gender { get; set; }

    public string? Dateofbirth { get; set; }

    public string Username { get; set; } = null!;

    public string Studentnumber { get; set; } = null!;

    public string Campusname { get; set; } = null!;

    public string Universityname { get; set; } = null!;
}