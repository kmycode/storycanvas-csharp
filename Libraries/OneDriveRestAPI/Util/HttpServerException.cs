using System;
using System.Runtime.Serialization;

namespace OneDriveRestAPI.Util
{
    public class HttpServerException : Exception
    {
		public int HR { get; set; }

		public int HttpCode { get; set; }

        public HttpServerException(string message) : base(message)
        {
        }

        public HttpServerException()
        {
        }

        public HttpServerException(string message, int hr) : base(message)
        {
			this.HR = hr;
        }

        public HttpServerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public HttpServerException(int httpCode, string message, Exception innerException) : base(message, innerException)
        {
			this.HttpCode = httpCode;
        }

        public HttpServerException(int httpCode, string message, int hr) : base(message)
		{
			this.HttpCode = httpCode;
			this.HR = hr;
		}

        public HttpServerException(int httpCode, string message) : base(message)
		{
			this.HttpCode = httpCode;
		}

        protected HttpServerException(SerializationInfo info, StreamingContext context) : base(/*info, context*/)
        {
        }

        public int? Attempts { get; set; }
    }
}