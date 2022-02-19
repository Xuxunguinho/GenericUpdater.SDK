using System;

namespace GenericUpdater.SDK
{
    public class NoInternetConnectionException : Exception
    {
        public NoInternetConnectionException(string message):base(message)
        {
            
        }
    }
}