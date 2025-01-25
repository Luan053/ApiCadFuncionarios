using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Model.CompanyAggregate;
using WebApi.Domain.Model.EmployeeAggregate;
using WebApi.Domain.Model.UserAggregate;

namespace WebApi.Infrastructure
{
    public class ConnectionContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ConnectionContext(DbContextOptions<ConnectionContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações específicas para PostgreSQL
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Usar snake_case para nomes de tabelas
                entity.SetTableName(entity.GetTableName().ToSnakeCase());

                // Usar snake_case para nomes de colunas
                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.GetColumnName().ToSnakeCase());
                }

                // Usar snake_case para nomes de chaves estrangeiras
                foreach (var key in entity.GetForeignKeys())
                {
                    key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
                }
            }

            // Configurações específicas de entidades
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Photo).IsRequired();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
                
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });
        }
    }

    // Extensão para converter para snake_case
    public static class StringExtensions
    {
        public static string ToSnakeCase(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var builder = new System.Text.StringBuilder(text.Length + Math.Min(2, text.Length / 5));
            var previousCategory = default(System.Globalization.UnicodeCategory?);

            for (var currentIndex = 0; currentIndex < text.Length; currentIndex++)
            {
                var currentChar = text[currentIndex];
                if (currentChar == '_')
                {
                    builder.Append('_');
                    previousCategory = null;
                    continue;
                }

                var currentCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(currentChar);
                switch (currentCategory)
                {
                    case System.Globalization.UnicodeCategory.UppercaseLetter:
                    case System.Globalization.UnicodeCategory.TitlecaseLetter:
                        if (previousCategory == System.Globalization.UnicodeCategory.SpaceSeparator ||
                            previousCategory == System.Globalization.UnicodeCategory.LowercaseLetter ||
                            previousCategory != null &&
                            currentIndex > 0 &&
                            currentIndex + 1 < text.Length &&
                            char.IsLower(text[currentIndex + 1]))
                        {
                            builder.Append('_');
                        }

                        currentChar = char.ToLower(currentChar);
                        break;

                    case System.Globalization.UnicodeCategory.LowercaseLetter:
                    case System.Globalization.UnicodeCategory.DecimalDigitNumber:
                        if (previousCategory == System.Globalization.UnicodeCategory.SpaceSeparator)
                        {
                            builder.Append('_');
                        }
                        break;

                    default:
                        if (previousCategory != null)
                        {
                            previousCategory = System.Globalization.UnicodeCategory.SpaceSeparator;
                        }
                        continue;
                }

                builder.Append(currentChar);
                previousCategory = currentCategory;
            }

            return builder.ToString();
        }
    }
}
