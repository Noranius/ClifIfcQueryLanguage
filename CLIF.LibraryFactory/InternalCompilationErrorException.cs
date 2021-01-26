using System;
using System.Collections.Generic;
using System.Text;

namespace CLIF.LibraryFactory
{
    /// <summary>
    /// Is thrown when there were internal errors during compilation
    /// </summary>
    public class InternalCompilationErrorException : Exception
    {
        public IEnumerable<string> InternalErrors { get; private set; }

        public InternalCompilationErrorException(IEnumerable<string> enumerable) : base("Internal exceptions on compilitaions")
        {
            this.InternalErrors = enumerable;
        }

        public override string ToString()
        {
            return base.ToString() + ". Internal errors: " + string.Join("\r\n", this.InternalErrors);
        }
    }
}
