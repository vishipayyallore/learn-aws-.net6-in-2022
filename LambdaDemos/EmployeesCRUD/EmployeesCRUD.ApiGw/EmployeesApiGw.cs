using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Employees.Core.Models;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EmployeesCRUD.ApiGw;

public class EmployeesApiGw
{

    public async Task<APIGatewayProxyResponse> GetEmployeeByIdHandler(APIGatewayProxyRequest apiGatewayProxyRequest, ILambdaContext context)
    {
        EmployeeResponseDto employeeResponseDto = new();
        APIGatewayProxyResponse apiGatewayProxyResponse = new();

        apiGatewayProxyRequest.PathParameters.TryGetValue("employeeId", out var employeeId);

        if (string.IsNullOrEmpty(employeeId))
        {
            context.Logger.LogDebug($"Did NOT receive the valid request for Employee.");

            employeeResponseDto.Message = "Received Invalid Employee Id";

            apiGatewayProxyResponse.Body = JsonSerializer.Serialize(employeeResponseDto);
            apiGatewayProxyResponse.StatusCode = 400;

            return apiGatewayProxyResponse;
        }

        context.Logger.LogDebug($"Received the request with Employee Id {employeeId}.");

        var _dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
        EmployeeDto employeeDto = await _dynamoDbContext.LoadAsync<EmployeeDto>(employeeId);


        context.Logger.LogDebug($"Sending the response for Employee Id {employeeId}.");

        employeeResponseDto = GenerateEmployeeResponseDto(employeeDto);
        
        apiGatewayProxyResponse.Body = JsonSerializer.Serialize(employeeResponseDto);

        apiGatewayProxyResponse.StatusCode = (employeeDto is not null) ? 200 : 404;

        return apiGatewayProxyResponse;
    }

    private static EmployeeResponseDto GenerateEmployeeResponseDto(EmployeeDto employeeDto)
    {
        EmployeeResponseDto employeeResponseDto = new();

        if (employeeDto is not null)
        {
            employeeResponseDto.Success = true;
            employeeResponseDto.Message = "Record Found";
            employeeResponseDto.Employee = employeeDto;
        }

        return employeeResponseDto;
    }

}
