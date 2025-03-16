using System;
namespace ModelLayer.model
{
	public class ResponseBody<T>
	{
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T Data { get; set; }
    }
}

