namespace CodeGeneration.CodeModifiers
{

    partial class ModifyImportsOptions
    {
        public ModifyImportsOptions()
        {
            AddImport = new ImportTypeCollection().Add(new ImportType());
            RemoveImport = new ImportTypeCollection().Add(new ImportType());
        }
    }

    public partial class ImportTypeCollection
    {
        public ImportTypeCollection() { }

        public new ImportTypeCollection Add(ImportType type) { base.Add(type); return this; }   
    }
    
    public partial class ImportType
    {
        public ImportType() { Name = ""; }
    }
}
