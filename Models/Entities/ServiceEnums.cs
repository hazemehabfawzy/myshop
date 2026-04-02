namespace TechVault.API.Models.Entities
{
    public enum ServiceType
    {
        ScreenReplacement,
        BatteryReplacement,
        ChargingPortRepair,
        WaterDamage,
        SoftwareReinstall,
        HardwareCleaning,
        RAMUpgrade,
        StorageUpgrade,
        KeyboardReplacement,
        CustomPCBuild,
        Other
    }

    public enum ServiceRequestStatus
    {
        Received,
        Diagnosing,
        WaitingParts,
        InProgress,
        ReadyForPickup,
        Completed,
        Cancelled
    }
}
