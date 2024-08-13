using System.ComponentModel.DataAnnotations;

namespace N5Challenge.Application.DTOs.Permission
{
    public class PermissionCreateDto
    {
        [Required]
        public string EmployeeName { get; set; }

        [Required]
        public string EmployeeLastName { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PermissionTypeId { get; set; }

        [Required]
        public DateTime PermissionDate { get; set; }
    }
}
