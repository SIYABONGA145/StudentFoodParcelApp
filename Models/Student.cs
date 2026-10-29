using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // Added for validations

namespace StudentFoodParcelApp.Models;

public partial class Student
{
    public int Studentid { get; set; }

    [Required(ErrorMessage = "Student number is required.")]
    [RegularExpression(@"^\d{9}$", ErrorMessage = "Student number must be exactly 9 digits (numbers only).")]
    public string Studentnumber { get; set; } = null!;

    public int? Campusid { get; set; }

    public int? Personid { get; set; }

    public virtual Campus? Campus { get; set; }

    public virtual ICollection<Fundingstatus> Fundingstatuses { get; set; } = new List<Fundingstatus>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Person? Person { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}