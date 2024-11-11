using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Windows.Forms;

namespace _3D_Braid
{
    public interface IFollowing
    {
        bool IsFollowing { get; set; }
    }

    public class FollowingBehavior
    {
        private bool _isFollowing = true;
        private readonly IGH_Param _param;

        public FollowingBehavior(IGH_Param param)
        {
            _param = param;
        }

        public bool IsFollowing
        {
            get { return _isFollowing; }
            set
            {
                if (_isFollowing != value)
                {
                    _isFollowing = value;
                    if (!_isFollowing && _param.Attributes != null)
                    {
                        _param.Attributes.Pivot = _param.Attributes.Pivot;
                    }
                }
            }
        }

        public void AddFollowMenuItem(ToolStripDropDown menu)
        {
            GH_DocumentObject.Menu_AppendSeparator(menu);
            GH_DocumentObject.Menu_AppendItem(menu, "Follow Component", Menu_FollowClicked, true, _isFollowing);
        }

        private void Menu_FollowClicked(object sender, EventArgs e)
        {
            IsFollowing = !IsFollowing;
            if (_param.OnPingDocument() != null)
            {
                _param.RecordUndoEvent("Toggle Following State");
            }
            Instances.ActiveCanvas.Document.NewSolution(false);
        }

        public void Write(GH_IO.Serialization.GH_IWriter writer)
        {
            writer.SetBoolean("IsFollowing", _isFollowing);
        }

        public void Read(GH_IO.Serialization.GH_IReader reader)
        {
            if (reader.ItemExists("IsFollowing"))
            {
                _isFollowing = reader.GetBoolean("IsFollowing");
            }
        }
    }
}