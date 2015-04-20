namespace Mirage.Urbanization.ZoneConsumption.Base
{
    public interface IConsumeAreaOperation
    {
        IGetCanOverrideWithResult CanOverrideWithResult { get; }
        void Apply();
        string Description { get; }
    }
}