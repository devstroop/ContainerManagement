using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace ContainerManagement.Components.Layout
{
    public partial class MainLayout
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }
        

        private bool sidebarExpanded = true;

        [Inject]
        protected ContainerManagement.DatabaseService DatabaseService { get; set; }

        private System.Linq.IQueryable<ContainerManagement.Models.Database.Node> _nodes;
        private Models.Database.Node _selectedNode;
        private string _selectedNodeName;

        [Inject]
        protected SecurityService Security { get; set; }

        private void SidebarToggleClick()
        {
            sidebarExpanded = !sidebarExpanded;
        }

        protected override async Task OnInitializedAsync()
        {
            _nodes = await DatabaseService.GetNodes();
        }

        private async System.Threading.Tasks.Task NodesSelectedItemChanged(object args)
        {
            if (args != null)
            {
                var node = args as ContainerManagement.Models.Database.Node;
                _selectedNode = _nodes.FirstOrDefault(n => n.Name == node.Name);
                NotificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Info, Summary = "Node selected",
                    Detail = $"Node {_selectedNode?.Name} selected."
                });
            }
            else
            {
                _selectedNode = null;
                NotificationService.Notify(new NotificationMessage()
                {
                    Severity = NotificationSeverity.Info, Summary = "Node deselected",
                    Detail = "Node deselected."
                });
            }
            StateHasChanged();
        }

        protected void ProfileMenuClick(RadzenProfileMenuItem args)
        {
            if (args.Value == "Logout")
            {
                Security.Logout();
            }
        }
    }
}
