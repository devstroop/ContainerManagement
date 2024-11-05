using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace ContainerManagement.Components.Pages.Administration.Nodes
{
    public partial class Nodes
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

        [Inject]
        public DatabaseService DatabaseService { get; set; }

        protected IEnumerable<ContainerManagement.Models.Database.Node> nodes;

        protected RadzenDataGrid<ContainerManagement.Models.Database.Node> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            nodes = await DatabaseService.GetNodes(new Query { Filter = $@"i => i.Name.Contains(@0) || i.DockerAPI.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            nodes = await DatabaseService.GetNodes(new Query { Filter = $@"i => i.Name.Contains(@0) || i.DockerAPI.Contains(@0)", FilterParameters = new object[] { search } });
            nodes = await DatabaseService.GetNodes();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddNode>("Add Node", null);
            await grid0.Reload();
        }

        protected async Task EditRow(ContainerManagement.Models.Database.Node args)
        {
            await DialogService.OpenAsync<EditNode>("Edit Node", new Dictionary<string, object> { {"Name", args.Name} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, ContainerManagement.Models.Database.Node node)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await DatabaseService.DeleteNode(node.Name);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Environment"
                });
            }
        }
    }
}