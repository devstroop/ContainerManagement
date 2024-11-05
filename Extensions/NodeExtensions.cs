using Docker.DotNet;
using Microsoft.AspNetCore.Components;

namespace ContainerManagement.Extensions;

public static class NodeExtensions
{
    public static DockerClient CreateClient(this Models.Database.Node node)
    {
        return new DockerClientConfiguration(new Uri(node.DockerAPI)).CreateClient();
    }
    
    
    public static bool IsSelected(this Models.Database.Node node, Models.Database.Node selectedNode)
    {
        return node == selectedNode;
    }
}