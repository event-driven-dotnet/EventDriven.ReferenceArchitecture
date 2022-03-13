namespace EventDriven.ReferenceArchitecture.Specs.Configuration;

public class ReferenceArchSpecsSettings
{
    public Guid Customer1Id { get; set; }
    public Guid Customer2Id { get; set; }
    public Guid Customer3Id { get; set; }
    public Guid CustomerPubSub1Id { get; set; }
    public Guid Order1Id { get; set; }
    public Guid Order2Id { get; set; }
    public Guid Order3Id { get; set; }
    public Guid Order4Id { get; set; }
    public Guid OrderPubSub1Id { get; set; }
    public Guid OrderPubSub2Id { get; set; }
    public string? CustomerServiceBaseAddress { get; set; }
    public string? OrderServiceBaseAddress { get; set; }
    public bool StartTyeProcess { get; set; }
    public TimeSpan TyeProcessTimeout { get; set; }
    public TimeSpan AddressUpdateTimeout { get; set; }
}