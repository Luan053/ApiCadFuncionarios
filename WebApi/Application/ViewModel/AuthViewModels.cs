using System.ComponentModel.DataAnnotations;

namespace WebApi.Application.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        public string Username { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        public string Password { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 100 caracteres")]
        public string Username { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 100 caracteres")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "As senhas não conferem")]
        public string ConfirmPassword { get; set; }
    }

    public class RefreshTokenViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }

    public class AuthResponseViewModel
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
