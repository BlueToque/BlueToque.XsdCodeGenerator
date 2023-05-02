using System.CodeDom;

namespace CodeGeneration.CodeModifiers
{
    /// <summary>
    /// When generating code for WCF services, all public properties are by default 
    /// serialized. This code modifier attempts to marks as NonSerialzied all events.
    /// </summary>
    public class AddNonSerializedEvents: BaseCodeModifier
    {
        public AddNonSerializedEvents() : base("Add Non Serialized Events") { }

        #region ICodeModifier Members

        public override void Execute(CodeNamespace codeNamespace)
        {
            // foreach datatype in the codeNamespace
            foreach (CodeTypeDeclaration type in codeNamespace.Types)
            {
                if (type.IsEnum) continue;

                foreach (CodeTypeMember member in type.Members)
                {
                    if (!(member is CodeMemberEvent codeEvent))
                        continue;

                    // add the non serialized attribute
                    codeEvent.CustomAttributes.Add(new CodeAttributeDeclaration("field: System.NonSerialized"));
                }
            }
        }

        #endregion
    }
}
