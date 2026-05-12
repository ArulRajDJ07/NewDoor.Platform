    using AutoMapper;
    using MediatR;
    using NewDoor.Platform.DTO.Features.Events.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Events.Query;

    namespace NewDoor.API.Features.Events.Handler
    {
        public class FindAllEventHandler : IRequestHandler<FindAllEventQuery, List<EventResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IEventRepository _eventRepository;

            public FindAllEventHandler(IMapper mapper, IEventRepository eventRepository)
            {
                _mapper = mapper;
                _eventRepository = eventRepository;
            }

            public async Task<List<EventResponse>> Handle(FindAllEventQuery request, CancellationToken cancellationToken)
            {
                var filter = request.Filter ?? new EventFilterRequest();
                var events = await _eventRepository.GetAllFilteredAsync(filter);
                return _mapper.Map<List<EventResponse>>(events);
            }
        }
    }