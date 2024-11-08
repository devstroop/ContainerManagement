using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace ContainerManagement.Components.Pages.Node
{
    public partial class Images
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
        
        [CascadingParameter] public Models.Database.Node SelectedNode { get; set; }
        private  IEnumerable<ImagesListResponse> ImagesListResponses { get; set; }
        private DockerClient _client;

        [Inject]
        protected SecurityService Security { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            if (SelectedNode != null)
            {
                _client = new DockerClientConfiguration(new Uri(SelectedNode.DockerAPI)).CreateClient();
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "Please select a node first.");
            }
        }
        
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                ImagesListResponses = await _client.Images.ListImagesAsync(new ImagesListParameters());
                await JSRuntime.InvokeVoidAsync("console.log", ImagesListResponses);
                StateHasChanged();
            }
        }
        
        private Task Search(ChangeEventArgs arg)
        {
            throw new NotImplementedException();
        }
    }
}