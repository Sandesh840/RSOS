using System.Net;

namespace Application.DTOs.Base;

public class ResponseDTO<T> where T : class
{
    public HttpStatusCode StatusCode { get; set; }

    public string Status { get; set; }

    public string Message { get; set; }
    public int LoginType { get; set; }

    public T? Result { get; set; }
}