using Grasshopper.Kernel;
using System;
using Grasshopper.Kernel.Parameters;
using System.Windows.Forms;

namespace _3D_Braid
{
    public class FollowingCurveParam : Param_Curve, IFollowing
    {
        private readonly FollowingBehavior _following;

        public FollowingCurveParam()
        {
            _following = new FollowingBehavior(this);
        }

        public bool IsFollowing
        {
            get { return _following.IsFollowing; }
            set { _following.IsFollowing = value; }
        }

        public override Guid ComponentGuid => new Guid("D71C9759-3B74-4BB7-9739-332448C21FAF");

        public override void AppendAdditionalMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalMenuItems(menu);
            _following.AddFollowMenuItem(menu);
        }

        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            bool result = base.Write(writer);
            _following.Write(writer);
            return result;
        }

        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            bool result = base.Read(reader);
            _following.Read(reader);
            return result;
        }
    }
}