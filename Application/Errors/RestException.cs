namespace Application.Errors
{
    using System;
    using System.Net;

    public class RestException : Exception
    {
        public RestException(HttpStatusCode code, object errors = null) 
        {
            Code = code;
            Errors = errors;
        }

        public HttpStatusCode Code { get; }
        
        public object Errors { get; }
    }
}