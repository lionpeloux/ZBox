using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace ZymeToolbox.Grasshopper
{
    public class ZymeToolbox_GrasshopperInfo : GH_AssemblyInfo
    {
        public override string Name => "ZymeToolbox.Grasshopper";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "A multipurpose toolset of components from Zyme Engineering company";

        public override Guid Id => new Guid("4725e37d-4efc-4a90-bf2b-a33182599198");

        //Return a string identifying you or your company.
        public override string AuthorName => "Lionel du Peloux";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "contact@zyme.fr";

        public override string Version => base.Version;
    }
}