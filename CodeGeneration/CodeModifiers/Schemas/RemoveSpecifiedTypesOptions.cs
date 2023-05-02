using System.Linq;

namespace CodeGeneration.CodeModifiers
{

    partial class RemoveSpecifiedTypesOptions
    {
        public RemoveSpecifiedTypesOptions() => Type = new ClassTypeCollection().Add(new ClassType());
    }

    partial class ClassTypeCollection
    {
        internal bool ContainsName(string name) => this.Any(classType => classType.Name == name);

        public new ClassTypeCollection Add(ClassType type) { base.Add(type); return this; }

    }

    public partial class ClassType
    {
        public ClassType() { Name = ""; }
    }
}
