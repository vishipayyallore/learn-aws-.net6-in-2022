using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.Core;
using AWSLambda.Employees.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambda.Employees;

public class EmployeeLambda
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<EmployeeResponseDto> EmployeeHandler(EmployeeRequestDto employeeRequestDto, ILambdaContext context)
    {
        context.Logger.LogDebug($"Received the request with Employee Id {employeeRequestDto.employeeId}.");

        var _dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
        EmployeeDto employeeDto = await _dynamoDbContext.LoadAsync<EmployeeDto>(employeeRequestDto.employeeId);

        context.Logger.LogDebug($"Sending the response for Employee Id {employeeRequestDto.employeeId}.");

        EmployeeResponseDto employeeResponseDto = GenerateEmployeeResponseDto(employeeDto);

        return employeeResponseDto;
    }

    private static EmployeeResponseDto GenerateEmployeeResponseDto(EmployeeDto employeeDto)
    {
        EmployeeResponseDto employeeResponseDto = new();
        if (employeeDto != null)
        {
            employeeResponseDto.Success = true;
            employeeResponseDto.Message = "Record Found";
            employeeResponseDto.Employee = employeeDto;
        }

        return employeeResponseDto;
    }
}
