using System.ComponentModel.DataAnnotations;

namespace ExamEquityCalculatorApp.Models
{
    public class EquityCalculator
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Selling Price:")]
        [DisplayFormat(DataFormatString = "{0:C}", ApplyFormatInEditMode = false)]
        public decimal SellingPrice { get; set; }

        [Required]
        [Display(Name = "Reservation Date:")]
        public DateTime ReservationDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than 0.")]
        [Display(Name = "Equity Term:")]
        public int EquityTerm { get; set; }

        

    }
}
