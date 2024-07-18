using System;

namespace dotnetapp.Exceptions
{
    public class WorkshopBookingException : Exception
    {
        public WorkshopBookingException(string message) : base(message) { }
    }
}