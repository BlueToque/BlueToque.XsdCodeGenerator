using System.Collections.Generic;

namespace CodeGeneration
{
    /// <summary>
    /// A list of assembly references
    /// Currently not used, could replace the stringcollection that is
    /// used.
    /// </summary>
    public class AssemblyReferenceCollection : List<string>
    {
        /// <summary>
        /// Don't add the same reference twice
        /// </summary>
        /// <param name="value"></param>
        public new AssemblyReferenceCollection Add(string value)
        {
            if (!this.Contains(value))
                base.Add(value);
            return this;
        }

    }
}
