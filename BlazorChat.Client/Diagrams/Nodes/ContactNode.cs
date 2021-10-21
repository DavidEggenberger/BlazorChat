using Blazor.Diagrams.Core.Models;
using BlazorChat.Client.Diagrams.Ports;
using BlazorChat.Shared;

namespace BlazorChat.Client.Diagrams.Nodes
{
    public class ContactNode : NodeModel
    {
        public ContactNode(Blazor.Diagrams.Core.Geometry.Point position = null) : base(position)
        {
            AddPort(new ColumnPort(this, PortAlignment.Bottom));
        }
        public UserDto UserDto { get; set; }
        private UserDto nac;
        public UserDto SelectedUser
        {
            get
            {
                return nac;
            }
            set
            {
                nac = value;
                Refresh();
            }
        }
    }
}
