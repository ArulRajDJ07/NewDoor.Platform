using System.Collections.Concurrent;
using NewDoor.DeviceSimulator.Models;

namespace NewDoor.DeviceSimulator.Services;

public class EventBufferService
{
    private readonly ConcurrentQueue<EventLogModel> _eventQueue = new();
    private const int MaxEvents = 100;

    public event Action? OnEventsUpdated;

    public void AddEvent(EventLogModel eventLog)
    {
        _eventQueue.Enqueue(eventLog);

        while (_eventQueue.Count > MaxEvents)
        {
            _eventQueue.TryDequeue(out _);
        }

        OnEventsUpdated?.Invoke();
    }

    public List<EventLogModel> GetLatestEvents()
    {
        return _eventQueue.Reverse().ToList();
    }

    public void Clear()
    {
        _eventQueue.Clear();
        OnEventsUpdated?.Invoke();
    }
}
