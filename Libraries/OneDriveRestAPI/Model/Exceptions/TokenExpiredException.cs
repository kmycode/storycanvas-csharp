using System;
using System.Runtime.Serialization;

namespace OneDriveRestAPI.Model.Exceptions
{
    public class TokenExpiredException : Exception
    {
        public TokenExpiredException()
        {
        }

        public TokenExpiredException(string message) : base(message)
        {
        }

        public TokenExpiredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TokenExpiredException(SerializationInfo info, StreamingContext context) : base(/*info, context*/)
        {
        }
    }
}

#if WINDOWS_UWP
namespace System.Runtime.Serialization
{
	public partial class SerializationInfo
	{
	}
}
#endif
