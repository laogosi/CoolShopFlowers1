using System.ComponentModel.DataAnnotations;

namespace CoolShopFlowers.Models.ViewModels
{
    public class UpdUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Поле не повинно пустувати")]
        [StringLength(100, ErrorMessage = "Ім'я не повинно перевищувати 100 символів")]
        public string? Name { get; set; }


        [Display(Name = "День народження")]
        [Required(ErrorMessage = "Будь ласка виберіть дату народження")]
        [Range(typeof(DateTime), "01/01/1900", "01/01/2010", ErrorMessage = "Дата народження повинна бути в межах від 01.01.1900 до 01.01.2010")]
        public DateTime BirthDay { get; set; }


        [Display(Name = "Formatted BirthDay")]
        public string FormattedBirthDay => BirthDay.ToShortDateString();


        [Required(ErrorMessage = "Поле не повинно пустувати")]
        public string Sex { get; set; }


        //Only for Admin|Moderator
        public bool IsBanned { get; set; } 
        public bool IsMuted {  get; set; }
    }
}
