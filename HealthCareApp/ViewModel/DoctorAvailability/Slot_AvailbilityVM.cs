using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Doctor
{
    public class Slot_AvailbilityVM
    {
        [Required(ErrorMessage = "Slot is required")]
        public int NewSlotId { get; set;}
        [Required(ErrorMessage = "Day is required")]

        public int AvailableId { get; set; }
        public int OldSlotId { get; set; }
    }
}
