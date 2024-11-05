using Docker.DotNet;
using Docker.DotNet.Models;

namespace ContainerManagement.Extensions;

public static class DockerClientExtensions
{
    public static async Task<VersionResponse> GetVersion(this DockerClient client)
    {
        return await client.System.GetVersionAsync();
    }
}