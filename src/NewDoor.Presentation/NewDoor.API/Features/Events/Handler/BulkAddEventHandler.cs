using AutoMapper;
using MediatR;
using NewDoor.API.Features.Events.Command;
using NewDoor.API.Repositories.Interface;
using NewDoor.Platform.Entities;

namespace NewDoor.API.Features.Events.Handler
{
    public class BulkAddEventHandler(IMapper mapper, IEventRepository eventRepository) 
        : IRequestHandler<BulkAddEventCommand, int>
    {
        public async Task<int> Handle(BulkAddEventCommand request, CancellationToken cancellationToken)
        {
            var events = request.eventRequest.eventList.Select(e => mapper.Map<Event>(e)).ToList();
            return await eventRepository.AddRangeAsync(events);
        }
    }
}
