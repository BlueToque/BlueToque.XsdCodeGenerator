using System.CodeDom;

namespace CodeGeneration.CodeModifiers
{
    public class RemoveObjectBase : BaseCodeModifier
    {
        public RemoveObjectBase() : base("Remove Object Base") { }

        /// <summary>
        /// Loop over all members and if it declares object as an explicit base
        /// class, remove the declaration
        /// </summary>
        /// <param name="codeNamespace"></param>
        public override void Execute(CodeNamespace codeNamespace)
        {
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {
                if (!type.IsClass)
                    continue;

                CodeTypeReference obj = null;
                foreach(CodeTypeReference baseType in type.BaseTypes)
                {
                    if (baseType.BaseType == "System.Object")
                        obj = baseType;
                }

                if (obj != null)
                    type.BaseTypes.Remove(obj);
            }
        }
    }
}
