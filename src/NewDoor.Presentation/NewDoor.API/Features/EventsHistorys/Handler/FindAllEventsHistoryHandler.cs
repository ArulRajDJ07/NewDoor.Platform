    using AutoMapper;
    using MediatR;
    using NewDoor.Platform.DTO.Features.EventsHistorys.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.EventsHistorys.Query;

    namespace NewDoor.API.Features.EventsHistorys.Handler
    {
        public class FindAllEventsHistoryHandler : IRequestHandler<FindAllEventsHistoryQuery, List<EventsHistoryResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IEventsHistoryRepository _eventsHistoryRepository;

            public FindAllEventsHistoryHandler(IMapper mapper, IEventsHistoryRepository eventsHistoryRepository)
            {
                _mapper = mapper;
                _eventsHistoryRepository = eventsHistoryRepository;
            }

            public async Task<List<EventsHistoryResponse>> Handle(FindAllEventsHistoryQuery request, CancellationToken cancellationToken)
            {
                var filter = request.Filter ?? new EventsHistoryFilterRequest();
                var eventsHistories = await _eventsHistoryRepository.GetAllFilteredAsync(filter);
                return _mapper.Map<List<EventsHistoryResponse>>(eventsHistories);
            }
        }
    }