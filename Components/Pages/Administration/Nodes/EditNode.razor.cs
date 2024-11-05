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
    public partial class EditNode
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

        [Parameter]
        public string Name { get; set; }

        protected override async Task OnInitializedAsync()
        {
            node = await DatabaseService.GetNodeByName(Name);
        }
        protected bool errorVisible;
        protected ContainerManagement.Models.Database.Node node;

        protected async Task FormSubmit()
        {
            try
            {
                await DatabaseService.UpdateNode(Name, node);
                DialogService.Close(node);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}