//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by the BlueToque XsdCodeGenerator tool.
//     Tool Version:    2.23.418.0
//     Runtime Version: 4.0.30319.42000
//     Generated on:    2023-04-19 18:39:42
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
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/ImportFixerOptions.Options")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/ImportFixerOptions.Options", IsNullable=false)]
    public partial class ImportFixerOptions : System.ComponentModel.INotifyPropertyChanged {
        
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
        private NamespaceTypeCollection namespaceField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Namespace")]
        public NamespaceTypeCollection Namespace {
            get {
                return this.namespaceField;
            }
            set {
                this.namespaceField = value;
                this.RaisePropertyChanged("Namespace");
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/ImportFixerOptions.Options")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/ImportFixerOptions.Options", IsNullable=true)]
    public partial class NamespaceType : System.ComponentModel.INotifyPropertyChanged {
        
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
        private string xmlNamespaceField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string XmlNamespace {
            get {
                return this.xmlNamespaceField;
            }
            set {
                this.xmlNamespaceField = value;
                this.RaisePropertyChanged("XmlNamespace");
            }
        }
        
        /// <summary />
        private string codeNamespaceField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string CodeNamespace {
            get {
                return this.codeNamespaceField;
            }
            set {
                this.codeNamespaceField = value;
                this.RaisePropertyChanged("CodeNamespace");
            }
        }
    }
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/ImportFixerOptions.Options")]
    public partial class NamespaceTypeCollection : System.Collections.Generic.List<NamespaceType> {
    }
}
#pragma warning restore 1591