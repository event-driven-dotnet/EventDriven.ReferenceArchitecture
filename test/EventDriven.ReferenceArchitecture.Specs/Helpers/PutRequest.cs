namespace EventDriven.ReferenceArchitecture.Specs.Helpers;

public class PutRequest
{
    public Guid Id { get; set; }
    public string ETag { get; set; } = null!;
}