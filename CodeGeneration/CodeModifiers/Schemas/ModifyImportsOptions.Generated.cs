//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by the BlueToque XsdCodeGenerator tool.
//     Tool Version:    2.23.418.0
//     Runtime Version: 4.0.30319.42000
//     Generated on:    2023-04-19 18:39:47
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated. For more information see http://BlueToque.ca
// </autogenerated>
//------------------------------------------------------------------------------

//	http://BlueToque.ca
// Workaround for bug http://lab.msdn.microsoft.com/productfeedback/viewfeedback.aspx?feedbackid=d457a36e-0ce8-4368-9a27-92762860d8e1
#pragma warning disable 1591
namespace CodeGeneration.CodeModifiers {
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/ModifyImports.Options")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/ModifyImports.Options", IsNullable=false)]
    public partial class ModifyImportsOptions : System.ComponentModel.INotifyPropertyChanged {
        
        /// <summary />
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        /// <summary />
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        
        /// <summary />
        private ImportTypeCollection addImportField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("AddImport")]
        public ImportTypeCollection AddImport {
            get {
                return this.addImportField;
            }
            set {
                this.addImportField = value;
                this.RaisePropertyChanged("AddImport");
            }
        }
        
        /// <summary />
        private ImportTypeCollection removeImportField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("RemoveImport")]
        public ImportTypeCollection RemoveImport {
            get {
                return this.removeImportField;
            }
            set {
                this.removeImportField = value;
                this.RaisePropertyChanged("RemoveImport");
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/ModifyImports.Options")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/ModifyImports.Options", IsNullable=true)]
    public partial class ImportType : System.ComponentModel.INotifyPropertyChanged {
        
        /// <summary />
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        /// <summary />
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        
        /// <summary />
        private string nameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
                this.RaisePropertyChanged("Name");
            }
        }
    }
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/ModifyImports.Options")]
    public partial class ImportTypeCollection : System.Collections.Generic.List<ImportType> {
    }
}
#pragma warning restore 1591