
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Demo.DAL.Models;

namespace Demo.PL.ViewModels
{
    public class EmployeeViewModel
    {
        public int Id { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Range(18, 60)]
        public int Age { get; set; }

        public string Address { get; set; } = string.Empty;

        [DataType(DataType.Currency)]
        public decimal Salary { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string Phone { get; set; } = string.Empty;

        public IFormFile? Image { get; set; }

        public string? ImageName { get; set; }

        public bool IsActive { get; set; }

        public Department? Department { get; set; }

        [Required(ErrorMessage = "Please select a department.")]
        public int? DepartmentId { get; set; }
    }
}
