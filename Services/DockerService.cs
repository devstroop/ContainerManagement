using Docker.DotNet;

namespace ContainerManagement;

public class DockerService(Uri uri)
{
    private readonly DockerClient _client = new DockerClientConfiguration(uri).CreateClient();
}