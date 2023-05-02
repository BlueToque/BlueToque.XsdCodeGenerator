using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeGeneration.CodeModifiers
{
    /// <summary>
    /// A strongly typed  collection of ICodeModifers
    /// </summary>
    public class CodeModifierCollection : List<ICodeModifier>
    {
        /// <summary>
        /// Remove the code modifier with the given type
        /// </summary>
        /// <param name="type"></param>
        /// <returns>True if successfull, false otherwise</returns>
        public bool Remove(Type type)
        {
            var modifier = this.FirstOrDefault(x => x.GetType() == type);
            return modifier != null && Remove(modifier);
        }

        /// <summary>
        /// Return true if the collection contains an element with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsName(string name) => this.Any(x => x.Name == name);

        /// <summary>
        /// Don't add the same code modifier twice
        /// </summary>
        /// <param name="modifier"></param>
        public new CodeModifierCollection Add(ICodeModifier modifier)
        {
            if (!ContainsName(modifier.Name))
                base.Add(modifier);
            return this;
        }

        /// <summary>
        /// Replace a code modifier if it already exists
        /// </summary>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public CodeModifierCollection Replace(ICodeModifier modifier)
        {
            if (ContainsName(modifier.Name))
                this.Remove(this.FirstOrDefault(x => x.Name == modifier.Name));
            base.Add(modifier);
            return this;
        }
    }
}
