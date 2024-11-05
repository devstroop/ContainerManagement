using System.Net.Http;
using Docker.DotNet;
using Docker.DotNet.Models;
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

namespace ContainerManagement.Components.Pages
{
    public partial class Index
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
        [CascadingParameter] public EventCallback<Models.Database.Node> NodeSelectionChange { get; set; }
        
        private DockerClient _client;
        
        private int _totalContainers = 0;
        private int _totalImages = 0;
        private int _totalVolumes = 0;
        private int _totalNetworks = 0;
        private int _totalStacks = 0;
        
        protected override async Task OnInitializedAsync()
        {
            NodeSelectionChange = new EventCallback<Models.Database.Node>(this, async (dynamic node) =>
            {
                SelectedNode = node;
                await LoadData();
            });
            await LoadData();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JSRuntime.InvokeVoidAsync("console.log", "Total Containers: " + _totalContainers);
                await JSRuntime.InvokeVoidAsync("console.log", "Total Images: " + _totalImages);
                await JSRuntime.InvokeVoidAsync("console.log", "Total Volumes: " + _totalVolumes);
                await JSRuntime.InvokeVoidAsync("console.log", "Total Networks: " + _totalNetworks);
                await JSRuntime.InvokeVoidAsync("console.log", "Total Stacks: " + _totalStacks);
            }
        }

        private async Task LoadData()
        {
            if (SelectedNode != null)
            {
                try
                {
                    _client = new DockerClientConfiguration(new Uri(SelectedNode.DockerAPI)).CreateClient();
                    if (_client != null)
                    {
                        _totalContainers = (await _client.Containers.ListContainersAsync(new ContainersListParameters())).Count;
                        _totalImages = (await _client.Images.ListImagesAsync(new ImagesListParameters())).Count;
                        _totalVolumes = (await _client.Volumes.ListAsync()).Volumes.Count;
                        _totalNetworks = (await _client.Networks.ListNetworksAsync()).Count;
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Error", ex.Message);
                }
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Warning", "Please select a node first.");
            }
        }
        
        
    }
}