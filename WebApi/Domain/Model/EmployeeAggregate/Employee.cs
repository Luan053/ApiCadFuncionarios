using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Domain.Model.EmployeeAggregate
{
    [Table("employee")]
    public class Employee
    {
        [Key]
        public int Id { get; private set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres")]
        public string Name { get; private set; }

        [Range(18, 100, ErrorMessage = "A idade deve estar entre 18 e 100 anos")]
        public int Age { get; private set; }

        [Required(ErrorMessage = "A foto é obrigatória")]
        public string Photo { get; private set; }

        [Required(ErrorMessage = "O cargo é obrigatório")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "O cargo deve ter entre 2 e 50 caracteres")]
        public string Role { get; private set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; private set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        protected Employee() { } // For EF Core

        public Employee(string name, int age, string photo, string role)
        {
            ValidateData(name, age, photo, role);

            Name = name;
            Age = age;
            Photo = photo;
            Role = role;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string name, int age, string photo, string role)
        {
            ValidateData(name, age, photo, role);

            Name = name;
            Age = age;
            Photo = photo;
            Role = role;
            UpdatedAt = DateTime.UtcNow;
        }

        private static void ValidateData(string name, int age, string photo, string role)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome é obrigatório", nameof(name));

            if (name.Length < 2 || name.Length > 100)
                throw new ArgumentException("O nome deve ter entre 2 e 100 caracteres", nameof(name));

            if (age < 18 || age > 100)
                throw new ArgumentException("A idade deve estar entre 18 e 100 anos", nameof(age));

            if (string.IsNullOrWhiteSpace(photo))
                throw new ArgumentException("A foto é obrigatória", nameof(photo));

            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("O cargo é obrigatório", nameof(role));

            if (role.Length < 2 || role.Length > 50)
                throw new ArgumentException("O cargo deve ter entre 2 e 50 caracteres", nameof(role));
        }
    }
}
