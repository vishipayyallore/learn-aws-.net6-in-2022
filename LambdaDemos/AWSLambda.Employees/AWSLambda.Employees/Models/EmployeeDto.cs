using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace AWSLambda.Employees.Models
{

    [DynamoDBTable("employees")]
    public class EmployeeDto
    {
        [DynamoDBHashKey]
        public string employeeId { get; set; } = string.Empty;

        public string name { get; set; } = string.Empty;

        public int age { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal salary { get; set; }

        public string designation { get; set; } = string.Empty;
    }
}
