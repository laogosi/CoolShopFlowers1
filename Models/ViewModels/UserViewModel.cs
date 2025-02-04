﻿using System.ComponentModel.DataAnnotations;

namespace CoolShopFlowers.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string? Name { get; set; }
        public string? UserName { get; set; }
        public int? Age { get; set; }

        [Display(Name = "День народження")]
        public DateTime BirthDay { get; set; }

        [Display(Name = "Formatted BirthDay")]
        public string FormattedBirthDay => BirthDay.ToShortDateString();
        public string Sex { get; set; }
        public bool IsBanned { get; set; }
        public bool IsMuted { get; set; }
    }
}
