using Blazor.Diagrams.Core.Models;
using BlazorMLSA.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMLSA.Client.Diagrams.Nodes
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
