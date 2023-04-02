using System.Net.Http;

namespace OpenAI;

public class OpenAIException : HttpRequestException
{
    public string MethodName { get; }
    public string Content { get; }
    public HttpResponseMessage Response { get; }
        
    public OpenAIException(string methodName, string content, HttpResponseMessage response)
    {
        MethodName = methodName;
        Content = content;
        Response = response;
    }

    public override string Message =>
        $"{MethodName} Failed! HTTP status code: {Response.StatusCode} | Response body: {Content}";
}