using System.ComponentModel.DataAnnotations;

namespace MultiTenancyFramework.Mvc
{
    public class ReLoginModel
    {
        [Required]
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
