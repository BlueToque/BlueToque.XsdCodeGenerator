using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace TestXsdCodeGeneration
{
    public partial class AssemblyTreeView : TreeView
    {
        public AssemblyTreeView() => InitializeComponent();

        public AssemblyTreeView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        Assembly m_assembly;

        Type[] m_types;

        string m_filter;

        const string IMAGE_KEY_ASSEMBLY = "Assembly";

        const string IMAGE_KEY_TYPE = "Type";

        const string IMAGE_KEY_METHOD = "Method";

        public event TreeViewEventHandler TypeSelected;

        public event TreeViewEventHandler MethodSelected;

        public void SetFilter(string filter)
        {
            m_filter = filter;
            UpdateTree();
        }

        public void SetAssembly(Assembly assembly)
        {
            m_assembly = assembly;
            m_types = m_assembly.GetTypes();
            Array.Sort(m_types, new TypeComparer());
            UpdateTree();
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            switch (e.Node.Tag)
            {
                case Assembly _: break;
                case Type _: TypeSelected?.Invoke(this,e); break;
                case MethodInfo _: TypeSelected?.Invoke(this, e); break;
            }
        }

        private void UpdateTree()
        {
            BeginUpdate();
            Nodes.Clear();
            if (m_types != null)
            {
                TreeNode root = new TreeNode(m_assembly.FullName) { Tag = m_assembly };
                SetTreeNodeImage(root);
                foreach (Type type in m_types)
                {
                    if (IsPassFilter(type))
                    {
                        TreeNode typeNode = new TreeNode(type.FullName) { Tag = type };
                        SetTreeNodeImage(typeNode);
                        root.Nodes.Add(typeNode);

                        foreach (MethodInfo method in type.GetRuntimeMethods())
                        {
                            TreeNode methodNode = new TreeNode(method.Name) { Tag = method };
                            SetTreeNodeImage(methodNode);
                            typeNode.Nodes.Add(methodNode);
                        }
                    }
                }
                Nodes.Add(root);
                root.Expand();
            }
            EndUpdate();
        }

        private bool IsPassFilter(Type type)
        {
            if (m_filter != null && !m_filter.Trim().Equals(""))
            {
                return type.FullName.ToUpper(CultureInfo.InvariantCulture).Contains(
                    m_filter.ToUpper(CultureInfo.InvariantCulture));
            }
            else
            {
                return true;
            }
        }

        private static void SetTreeNodeImage(TreeNode treeNode)
        {
            treeNode.ImageKey = GetTreeNodeImageKey(treeNode);
            treeNode.SelectedImageKey = treeNode.ImageKey;
            treeNode.StateImageKey = treeNode.ImageKey;
        }

        private static string GetTreeNodeImageKey(TreeNode treeNode)
        {
            switch (treeNode.Tag)
            {
                case Assembly _: return IMAGE_KEY_ASSEMBLY;
                case Type _: return IMAGE_KEY_TYPE;
                case MethodInfo _: return IMAGE_KEY_METHOD;
                default: return null;
            }
        }
    }

    public class TypeComparer : IComparer<Type>
    {
        #region IComparer<Type> Members

        public int Compare(Type x, Type y)
        {
            return string.Compare(x.FullName, y.FullName,
                StringComparison.Ordinal);
        }

        #endregion
    }
}
