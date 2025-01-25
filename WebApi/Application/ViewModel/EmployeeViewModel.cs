using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace WebApi.Application.ViewModel
{
    public class EmployeeViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A idade é obrigatória")]
        [Range(18, 100, ErrorMessage = "A idade deve estar entre 18 e 100 anos")]
        public int Age { get; set; }

        [Required(ErrorMessage = "A foto é obrigatória")]
        public IFormFile Photo { get; set; }

        [Required(ErrorMessage = "O cargo é obrigatório")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O cargo deve ter entre 2 e 50 caracteres")]
        public string Role { get; set; }
    }
}
