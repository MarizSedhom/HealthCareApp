using System.ComponentModel.DataAnnotations;

namespace HealthCareApp.ViewModel.Doctor
{
    public class Item<T,T2>
    {
        [Required(ErrorMessage ="Must Select Option")]
        public T Id { get; set; }
        public T2 Name { get; set; }
    }
}
