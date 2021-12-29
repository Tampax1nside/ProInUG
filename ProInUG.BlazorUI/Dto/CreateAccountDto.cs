using System.ComponentModel.DataAnnotations;

namespace ProInUG.BlazorUI.Dto
{
    public class CreateAccountDto
    {
        [StringLength(256, MinimumLength = 8)] public string Name { get; set; } = "";

        public Roles Role { get; set; }

        public string Description { get; set; } = "";

        [StringLength(256, MinimumLength = 8)] public string Password { get; set; } = "";
    }
}