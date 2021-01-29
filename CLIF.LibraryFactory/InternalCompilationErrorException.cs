using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CLIF.LibraryFactory
{
    /// <summary>
    /// Is thrown when there were internal errors during compilation
    /// </summary>
    public class InternalCompilationErrorException : Exception
    {
        public IReadOnlyList<string> InternalErrors { get; private set; }

        public InternalCompilationErrorException(IEnumerable<string> internalErrors) : base("Internal exceptions on compilitaions")
        {
            this.InternalErrors = internalErrors.ToList().AsReadOnly();
        }

        public override string ToString()
        {
            return base.ToString() + ". Internal errors: " + string.Join("\r\n", this.InternalErrors);
        }
    }
}
