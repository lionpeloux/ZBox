using Eto.Drawing;
using Eto.Forms;
using Grasshopper.GUI.Canvas;
using Grasshopper.GUI;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using System;
using System.Reflection.Emit;
using Rhino.UI;
using Rhino.Commands;

namespace ZymeToolbox.Climat.Grasshopper.Components
{
    public class GHComp_EtoForms_SimpleDialog : GH_Component
    {
        protected override System.Drawing.Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("{664C39F8-D05A-4E53-8937-77A4FB913BEE}");
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public SimpleDialog dlg;

        public GHComp_EtoForms_SimpleDialog()
          : base("Simple Dialog", "Simple Dialog", "Simple Dialog", "ZBox", "3 | EtoForms")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("input", "input", "desc", GH_ParamAccess.item, true);
            pManager[0].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool input = false;
            DA.GetData(0, ref input);
            
        }

        public override void CreateAttributes()
        {
            m_attributes = new CustomAttributes(this);
        }

        private class CustomAttributes : GH_ComponentAttributes
        {

            public CustomAttributes(IGH_Component component) : base(component) { }
            public override GH_ObjectResponse RespondToMouseDoubleClick(GH_Canvas sender, GH_CanvasMouseEvent e)
            {
                ((GHComp_EtoForms_SimpleDialog)Owner).ShowMainWindow();
                return GH_ObjectResponse.Handled;
            }
        }

        public void ShowMainWindow()
        {
            Rhino.RhinoApp.WriteLine("double click");
            //if (dlg == null || dlg.Visible == false )
            //{
            //    Application.Instance.Run();
            //    dlg = new SimpleDialog();
            //    //Application.Instance.Run(dlg);
            //}

            //GH_DocumentEditor owner = Grasshopper.Instances.DocumentEditor;
            //WindowInteropHelper helper = new WindowInteropHelper(MyMainWindow);
            //helper.Owner = owner.Handle;
            var dialog = new SimpleDialog();
            dialog.ShowModal();
            //MyMainWindow.Show();
            //new Application().Run(new SimpleDialog());
        }
    }

    public class SimpleDialog : Dialog
    {
        public SimpleDialog()
        {
            Title = "My Collapsible Eto Form";
            Resizable = false;
            Padding = new Padding(5);
            // Set ClientSize instead of Size, as each platform has different window border sizes
            ClientSize = new Size(600, 400);
            //Content = new Label { Text = "Some content", VerticalAlignment = VerticalAlignment.Center, TextAlignment = TextAlignment.Center };

            // button to toggle collapsing

        }

       
    }
}