using System.ComponentModel.DataAnnotations;

namespace ProInUG.BlazorUI.Models
{
    public class Credentials
    {
        [Required]
        public string Username { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";
    }
}
