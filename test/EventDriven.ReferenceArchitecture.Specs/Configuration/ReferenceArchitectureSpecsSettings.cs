namespace EventDriven.ReferenceArchitecture.Specs.Configuration;

public class ReferenceArchitectureSpecsSettings
{
    public Guid CustomerId { get; set; }
    public Guid Order1Id { get; set; }
    public Guid Order2Id { get; set; }
    public string? CustomerBaseAddress { get; set; }
    public bool StartTyeProcess { get; set; }
    public TimeSpan TyeProcessTimeout { get; set; }
    public TimeSpan AddressUpdateTimeout { get; set; }
}