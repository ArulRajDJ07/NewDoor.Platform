namespace NewDoor.Listener.Services;

public interface IEventEnrichmentService
{
    string DetermineEventCategory(string eventType);
    string DeterminePipeline(string eventCategory);
    string DeterminePriority(string eventType, double smokeLevel, double temperature);
}
