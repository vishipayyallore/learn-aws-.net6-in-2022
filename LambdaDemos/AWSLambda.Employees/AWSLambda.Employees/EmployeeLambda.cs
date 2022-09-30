using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using AWSLambda.Employees.Models;
using System.Text.Json;

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
    public async Task<APIGatewayProxyResponse> EmployeeHandler(APIGatewayProxyRequest aPIGatewayProxyRequest, ILambdaContext context)
    {
        APIGatewayProxyResponse aPIGatewayProxyResponse = new();
        EmployeeRequestDto employeeRequestDto = JsonSerializer.Deserialize<EmployeeRequestDto>(aPIGatewayProxyRequest.Body);
        EmployeeResponseDto employeeResponseDto = new();

        if (employeeRequestDto == null || string.IsNullOrEmpty(employeeRequestDto.employeeId))
        {
            context.Logger.LogDebug($"Did NOT receive the valid request for Employee.");

            employeeResponseDto.Message = "Received Invalid Employee Id";

            aPIGatewayProxyResponse.Body = JsonSerializer.Serialize(employeeResponseDto);
            aPIGatewayProxyResponse.StatusCode = 400;
            return aPIGatewayProxyResponse;
        }

        context.Logger.LogDebug($"Received the request with Employee Id {employeeRequestDto.employeeId}.");

        var _dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
        EmployeeDto employeeDto = await _dynamoDbContext.LoadAsync<EmployeeDto>(employeeRequestDto.employeeId);

        context.Logger.LogDebug($"Sending the response for Employee Id {employeeRequestDto.employeeId}.");

        employeeResponseDto = GenerateEmployeeResponseDto(employeeDto);

        aPIGatewayProxyResponse.Body = JsonSerializer.Serialize(employeeResponseDto);
        aPIGatewayProxyResponse.StatusCode = 200;
        return aPIGatewayProxyResponse;
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
