//------------------------------------------------------------------------------
// <autogenerated>
//     This code was generated by the XsdCodeGenerator tool.
//     Tool Version:    2.23.427
//     Runtime Version: 4.0.30319.42000
//     Generated on:    2023-04-28 16:15:23
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated. For more information see http://BlueToque.ca
// </autogenerated>
//------------------------------------------------------------------------------

//	http://BlueToque.ca
// Workaround for bug http://lab.msdn.microsoft.com/productfeedback/viewfeedback.aspx?feedbackid=d457a36e-0ce8-4368-9a27-92762860d8e1
#pragma warning disable 1591
namespace XsdCodeGenerator.Test.Schemas {
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9037.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/Test.BlueToque.Utility.Example.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://BlueToque.ca/Test.BlueToque.Utility.Example.xsd", IsNullable=true)]
    public partial class Example : System.ComponentModel.INotifyPropertyChanged {
        
        /// <summary />
        [field: System.NonSerialized()]
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        /// <summary />
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        
        /// <summary />
        private ItemDataCollection itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Item", IsNullable=false)]
        public ItemDataCollection Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
                this.RaisePropertyChanged("Items");
            }
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(ItemDataWithChildren))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9037.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/Test.BlueToque.Utility.Example.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://BlueToque.ca/Test.BlueToque.Utility.Example.xsd", IsNullable=true)]
    public partial class ItemData : System.ComponentModel.INotifyPropertyChanged {
        
        /// <summary />
        [field: System.NonSerialized()]
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        /// <summary />
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        
        /// <summary />
        [System.NonSerialized()]
        private System.Xml.XmlElement anyField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement Any {
            get {
                return this.anyField;
            }
            set {
                this.anyField = value;
                this.RaisePropertyChanged("Any");
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
        
        /// <summary />
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
                this.RaisePropertyChanged("Value");
            }
        }
        
        /// <summary />
        private bool visibleField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Visible {
            get {
                return this.visibleField;
            }
            set {
                this.visibleField = value;
                this.RaisePropertyChanged("Visible");
            }
        }
        
        /// <summary />
        private bool childField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Child {
            get {
                return this.childField;
            }
            set {
                this.childField = value;
                this.RaisePropertyChanged("Child");
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9037.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/Test.BlueToque.Utility.Example.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://BlueToque.ca/Test.BlueToque.Utility.Example.xsd", IsNullable=true)]
    public partial class ItemDataWithChildren : ItemData {
        
        /// <summary />
        private DataTypeCollection dataCollectionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Data", Namespace="http://BlueToque.ca/Example/Data", IsNullable=false)]
        public DataTypeCollection DataCollection {
            get {
                return this.dataCollectionField;
            }
            set {
                this.dataCollectionField = value;
                this.RaisePropertyChanged("DataCollection");
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.8.9037.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/Test.BlueToque.Utility.Example.xsd")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://BlueToque.ca/Test.BlueToque.Utility.Example.xsd", IsNullable=true)]
    public partial class ItemsType : System.ComponentModel.INotifyPropertyChanged {
        
        /// <summary />
        [field: System.NonSerialized()]
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        /// <summary />
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
        
        /// <summary />
        private ItemDataCollection itemField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Item")]
        public ItemDataCollection Item {
            get {
                return this.itemField;
            }
            set {
                this.itemField = value;
                this.RaisePropertyChanged("Item");
            }
        }
    }
    
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://BlueToque.ca/Test.BlueToque.Utility.Example.xsd")]
    public partial class ItemDataCollection : System.Collections.Generic.List<ItemData> {
    }
}
#pragma warning restore 1591