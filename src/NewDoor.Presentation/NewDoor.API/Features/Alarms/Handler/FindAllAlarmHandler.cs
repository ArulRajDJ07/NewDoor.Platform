    using AutoMapper;
    using MediatR;
    using NewDoor.Platform.DTO.Features.Alarms.Models;
    using NewDoor.Platform.Entities;
    using NewDoor.API.Repositories.Interface;
    using NewDoor.API.Features.Alarms.Query;

    namespace NewDoor.API.Features.Alarms.Handler
    {
        public class FindAllAlarmHandler : IRequestHandler<FindAllAlarmQuery, List<AlarmResponse>>
        {
            private readonly IMapper _mapper;
            private readonly IAlarmRepository _alarmRepository;

            public FindAllAlarmHandler(IMapper mapper, IAlarmRepository alarmRepository)
            {
                _mapper = mapper;
                _alarmRepository = alarmRepository;
            }

            public async Task<List<AlarmResponse>> Handle(FindAllAlarmQuery request, CancellationToken cancellationToken)
            {
                var filter = request.Filter ?? new AlarmFilterRequest();
                var alarms = await _alarmRepository.GetAllFilteredAsync(filter);
                return _mapper.Map<List<AlarmResponse>>(alarms);
            }
        }
    }