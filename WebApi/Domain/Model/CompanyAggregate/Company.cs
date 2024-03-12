using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApi.Domain.Model.CompanyAggregate
{
    [Table("company")]
    public class Company
    {
        [Key]
        public int id { get; set; }

        public string nameCompany { get; set; }

        public Company(string nameCompany)
        {
            this.nameCompany = nameCompany ?? throw new ArgumentNullException(nameof(nameCompany));
        }

    }
}
