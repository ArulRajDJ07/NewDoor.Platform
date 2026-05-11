using AutoMapper;
using MediatR;
using NewDoor.API.Features.EventsHistorys.Command;
using NewDoor.API.Repositories.Interface;
using NewDoor.Platform.Entities;

namespace NewDoor.API.Features.EventsHistorys.Handler
{
    public class BulkAddEventsHistoryHandler(IMapper mapper, IEventsHistoryRepository eventsHistoryRepository) 
        : IRequestHandler<BulkAddEventsHistoryCommand, int>
    {
        public async Task<int> Handle(BulkAddEventsHistoryCommand request, CancellationToken cancellationToken)
        {
            var eventsHistories = request.eventsHistoryRequest.eventsHistoryList.Select(e => mapper.Map<EventsHistory>(e)).ToList();
            return await eventsHistoryRepository.AddRangeAsync(eventsHistories);
        }
    }
}
