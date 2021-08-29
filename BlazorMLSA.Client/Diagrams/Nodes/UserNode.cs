using Blazor.Diagrams.Core.Models;
using BlazorMLSA.Client.Diagrams.Ports;
using BlazorMLSA.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMLSA.Client.Diagrams.Nodes
{
    public class UserNode : NodeModel
    {
        public UserNode(Blazor.Diagrams.Core.Geometry.Point position = null) : base(position)
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
