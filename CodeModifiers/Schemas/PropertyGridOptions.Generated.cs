//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by the BlueToque XsdCodeGenerator tool.
//     Tool Version:    2.23.418.0
//     Runtime Version: 4.0.30319.42000
//     Generated on:    2023-04-19 18:37:49
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated. For more information see http://BlueToque.ca
// </autogenerated>
//------------------------------------------------------------------------------

//	http://BlueToque.ca
// Workaround for bug http://lab.msdn.microsoft.com/productfeedback/viewfeedback.aspx?feedbackid=d457a36e-0ce8-4368-9a27-92762860d8e1
#pragma warning disable 1591
namespace BlueToque.XmlLibrary.CodeModifiers.Schemas {
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/PropertyGrid.Options")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/PropertyGrid.Options", IsNullable=false)]
    public partial class PropertyGridOptions : System.ComponentModel.INotifyPropertyChanged {
        
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
        private PropertyGridTypeCollection propertyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Property")]
        public PropertyGridTypeCollection Property {
            get {
                return this.propertyField;
            }
            set {
                this.propertyField = value;
                this.RaisePropertyChanged("Property");
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.4084.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/PropertyGrid.Options")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/PropertyGrid.Options", IsNullable=true)]
    public partial class PropertyGridType : PropertyType {
        
        /// <summary />
        private string displayNameField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DisplayName {
            get {
                return this.displayNameField;
            }
            set {
                this.displayNameField = value;
                this.RaisePropertyChanged("DisplayName");
            }
        }
        
        /// <summary />
        private string categoryField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Category {
            get {
                return this.categoryField;
            }
            set {
                this.categoryField = value;
                this.RaisePropertyChanged("Category");
            }
        }
        
        /// <summary />
        private string descriptionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
                this.RaisePropertyChanged("Description");
            }
        }
        
        /// <summary />
        private string editorField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Editor {
            get {
                return this.editorField;
            }
            set {
                this.editorField = value;
                this.RaisePropertyChanged("Editor");
            }
        }
        
        /// <summary />
        private bool browsableField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Browsable {
            get {
                return this.browsableField;
            }
            set {
                this.browsableField = value;
                this.RaisePropertyChanged("Browsable");
            }
        }
        
        /// <summary />
        private bool browsableFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool BrowsableSpecified {
            get {
                return this.browsableFieldSpecified;
            }
            set {
                this.browsableFieldSpecified = value;
                this.RaisePropertyChanged("BrowsableSpecified");
            }
        }
        
        /// <summary />
        private bool readOnlyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool ReadOnly {
            get {
                return this.readOnlyField;
            }
            set {
                this.readOnlyField = value;
                this.RaisePropertyChanged("ReadOnly");
            }
        }
        
        /// <summary />
        private bool readOnlyFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ReadOnlySpecified {
            get {
                return this.readOnlyFieldSpecified;
            }
            set {
                this.readOnlyFieldSpecified = value;
                this.RaisePropertyChanged("ReadOnlySpecified");
            }
        }
    }
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/CodeGeneration/CodeModifiers/PropertyGrid.Options")]
    public partial class PropertyGridTypeCollection : System.Collections.Generic.List<PropertyGridType> {
    }
}
#pragma warning restore 1591