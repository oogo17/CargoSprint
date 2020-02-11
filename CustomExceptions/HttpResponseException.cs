using System;
using System.Net;
using System.Runtime.Serialization;

namespace cargoSprint.API.CustomExceptions
{
    [Serializable]
    public class HttpResponseException : Exception
    {
        private HttpStatusCode notFound;

        public HttpResponseException()
        {
            
        }

        public HttpResponseException(System.Net.Http.HttpResponseMessage resp)
        {
        }

        public HttpResponseException(HttpStatusCode notFound)
        {
            this.notFound = notFound;
        }

        public HttpResponseException(string message) : base(message)
        {
        }

        public HttpResponseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected HttpResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}