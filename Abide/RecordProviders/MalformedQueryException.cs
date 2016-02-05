using System;

namespace Abide
{
    public class MalformedQueryException : Exception
    {
        public MalformedQueryException(string s) : base(s)
        {
            
        }
    }
}