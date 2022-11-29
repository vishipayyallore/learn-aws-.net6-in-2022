namespace Employees.Core.Models
{

    public class EmployeeResponseDto
    {
        public bool Success { get; set; }

        public string Message { get; set; } = "Record NOT Found";

        public EmployeeDto Employee { get; set; } = new EmployeeDto();
    }

}
