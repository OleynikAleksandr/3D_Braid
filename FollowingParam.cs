﻿using Grasshopper.Kernel;
using System;
using Grasshopper.Kernel.Parameters;
using System.Windows.Forms;

namespace _3D_Braid
{
    public class FollowingParam : Param_Number, IFollowing
    {
        private readonly FollowingBehavior _following;

        public FollowingParam()
        {
            _following = new FollowingBehavior(this);
        }

        public bool IsFollowing
        {
            get { return _following.IsFollowing; }
            set { _following.IsFollowing = value; }
        }

        public override Guid ComponentGuid => new Guid("C98CC970-E2B5-4D81-93A4-B4932353C86C");

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