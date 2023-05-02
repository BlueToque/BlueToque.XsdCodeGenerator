using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

namespace TestXsdCodeGeneration
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
            Application.Run(new MainForm());
        }

        //static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        //{
        //    AssemblyResolveForm form = new AssemblyResolveForm();
        //    form.AssemblyName = args.Name;

        //    if (form.ShowDialog() != DialogResult.OK)
        //        return null;

        //    try
        //    {
        //        Assembly asm = Assembly.LoadFrom(form.AssemblyName);
        //        return asm;
        //    }
        //    catch (Exception ex)
        //    {
        //        Trace.TraceError(ex.ToString());
        //        return null;
        //    }
                
        //}
    }
}