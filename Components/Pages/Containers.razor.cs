using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace ContainerManagement.Components.Pages;

public partial class Containers
{
    
    [Inject]
    private DatabaseService DatabaseService { get; set; }
    [Inject]
    private NavigationManager NavigationManager { get; set; }
    [Inject]
    private NotificationService NotificationService { get; set; }
    [Inject]
    private IJSRuntime JSRuntime { get; set; }
    
    [CascadingParameter] public Models.Database.Node SelectedNode { get; set; }
    
    private IEnumerable<ContainerListResponse> ContainerListResponses { get; set; }
    
    private DockerClient _client;
    protected override async Task OnInitializedAsync()
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
            ContainerListResponses = await _client.Containers.ListContainersAsync(new ContainersListParameters());
            await JSRuntime.InvokeVoidAsync("console.log", ContainerListResponses);
            StateHasChanged();
        }
    }

    private Task Search(ChangeEventArgs arg)
    {
        throw new NotImplementedException();
    }
}