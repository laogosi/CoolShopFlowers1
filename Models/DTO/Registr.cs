using System.ComponentModel.DataAnnotations;

namespace CoolShopFlowers.Models.DTO
{
    public class Registr
    {
        [Required]
        public string? Name { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        [RegularExpression("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "Мінімальна довжина 6, в слові повинні бути щонайменше:  1 Вел. буква, 1 мал. буква, 1 спец. символ і 1 цифра")]
        public string? Password { get; set; }

        [Required]
        [Compare("Password")]
        public string? PasswordConfirm { get; set; }

        [Required(ErrorMessage = "Будь ласка виберіть дату народження")]
        [Range(typeof(DateTime), "01/01/1904", "01/01/2014", ErrorMessage = "Дата народження повинна бути в межах від 01.01.1904 до 01.01.2014")]
        public DateTime BirthDay { get; set; }


        [Required(ErrorMessage = "Виберіть стать")]
        public string? Sex { get; set; }

        public string? Role { get; set; }
        public bool IsBanned { get; set; }
        public bool IsMuted { get; set; }
    }
}
