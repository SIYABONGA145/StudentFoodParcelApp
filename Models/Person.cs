using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Added for validations

namespace StudentFoodParcelApp.Models;

public partial class Person
{
    public int Personid { get; set; }

    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string? Firstname { get; set; }

    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string? Lastname { get; set; }

    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    public string? Emailaddress { get; set; }

    [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits (numbers only).")]
    public string? Phonenumber { get; set; }

    public string? Gender { get; set; }

    // SQL: date
    public DateOnly? Dateofbirth { get; set; }

    public string? Accountstatus { get; set; }

    // SQL: datetime
    public DateTime? Createddate { get; set; }
    public DateTime? Lastlogin { get; set; }

    public string Username { get; set; } = null!;
    public string? Password { get; set; }

    public virtual ICollection<Financestaff> Financestaffs { get; set; } = new List<Financestaff>();
    public virtual ICollection<Itadmin> Itadmins { get; set; } = new List<Itadmin>();
    public virtual ICollection<Srcadmin> Srcadmins { get; set; } = new List<Srcadmin>();
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}