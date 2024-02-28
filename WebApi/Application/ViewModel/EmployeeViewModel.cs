namespace WebApi.Application.ViewModel
{
    public class EmployeeViewModel
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public IFormFile? Photo { get; set; }
        public string? Role { get; set; }


    }
}
