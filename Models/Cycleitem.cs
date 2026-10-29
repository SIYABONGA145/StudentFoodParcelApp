using System;
using System.Collections.Generic;

namespace StudentFoodParcelApp.Models;

public partial class Cycleitem
{
    public int Cycleitemid { get; set; }

    public int? Cycleid { get; set; }

    public string Itemname { get; set; } = null!;

    public string? Quantity { get; set; }

    public virtual Foodparcelcycle? Cycle { get; set; }
}