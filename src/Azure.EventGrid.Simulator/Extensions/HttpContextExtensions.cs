using System.Net;
using Azure.EventGrid.Simulator.Models;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Azure.EventGrid.Simulator.Extensions;

public static class HttpContextExtensions
{
    public static async Task<string> RequestBody(this HttpContext context)
    {
        try
        {
            var reader = new StreamReader(context.Request.Body);
            //reader.BaseStream.Seek(0, SeekOrigin.Begin);
            return await reader.ReadToEndAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task WriteErrorResponse(this HttpContext context, HttpStatusCode statusCode, string errorMessage, string code)
    {
        var error = new ErrorMessage(statusCode, errorMessage, code);

        context.Response.Headers.Add(HeaderNames.ContentType, "application/json");

        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonConvert.SerializeObject(error, Formatting.Indented));
    }
}