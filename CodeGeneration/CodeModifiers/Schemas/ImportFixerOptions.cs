using System.Linq;

namespace CodeGeneration.CodeModifiers
{

    partial class ImportFixerOptions
    {
        public ImportFixerOptions() => Namespace = new NamespaceTypeCollection().Add(new NamespaceType());
    }

    partial class NamespaceTypeCollection
    {
        internal bool ContainsXmlName(string name) => this.Any(ns => ns.XmlNamespace == name);

        public new NamespaceTypeCollection Add(NamespaceType type) { base.Add(type); return this; }
    }

    public partial class NamespaceType
    {
        public NamespaceType() { CodeNamespace = ""; XmlNamespace = ""; }
    }
}
