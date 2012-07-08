namespace Utilities.Exceptions
{
    using System;
    using System.Reflection;

    public class RegexException : Exception
    {
        public RegexException(MethodBase method) : base(method.Name + " Error")
        {
        }
    }
}