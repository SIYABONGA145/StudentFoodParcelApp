using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentFoodParcelApp.Models
{
    public class ParcelCycleViewModel
    {
        public int CycleId { get; set; }

        [Required(ErrorMessage = "Please enter a food parcel cycle name.")]
        [StringLength(100, ErrorMessage = "The cycle name is too long. Please shorten it.")]
        public string CycleName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a valid start date.")]
        [Display(Name = "Start Date")]
        public string StartDate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a valid end date.")]
        [Display(Name = "End Date")]
        public string EndDate { get; set; } = string.Empty;

        public string DistributionPoint { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;

        public List<ParcelItemViewModel> Items { get; set; } = new List<ParcelItemViewModel>();
    }

    public class ParcelItemViewModel
    {
        public int ItemId { get; set; }
        public int CycleId { get; set; }

        [Required(ErrorMessage = "Please enter a food parcel item name.")]
        [StringLength(100, ErrorMessage = "The item name is too long. Please shorten it.")]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a valid quantity.")]
        [Range(1, 1000, ErrorMessage = "Please enter a whole number greater than zero.")]
        public int Quantity { get; set; } = 1;
    }

    public class AddItemRequestModel
    {
        [Required(ErrorMessage = "The selected food parcel cycle could not be found.")]
        public int CycleId { get; set; }

        [Required(ErrorMessage = "Please enter a food parcel item name.")]
        [StringLength(100, ErrorMessage = "The item name is too long. Please shorten it.")]
        public string ItemName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter a valid quantity.")]
        [Range(1, 1000, ErrorMessage = "Please enter a whole number greater than zero.")]
        public int Quantity { get; set; } = 1;
    }
}