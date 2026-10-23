namespace waypoint_server.Services;

public class CurrentUser
{
    public int? Id { get; set; }
    public string? Role { get; set; }
    public bool IsAuthenticated { get; set; }
}