using AutoMapper;
using MediatR;
using NewDoor.API.Features.Alarms.Command;
using NewDoor.API.Repositories.Interface;
using NewDoor.Platform.Entities;

namespace NewDoor.API.Features.Alarms.Handler
{
    public class BulkAddAlarmHandler(IMapper mapper, IAlarmRepository alarmRepository) 
        : IRequestHandler<BulkAddAlarmCommand, int>
    {
        public async Task<int> Handle(BulkAddAlarmCommand request, CancellationToken cancellationToken)
        {
            var alarms = request.alarmRequest.alarmList.Select(a => mapper.Map<Alarm>(a)).ToList();
            return await alarmRepository.AddRangeAsync(alarms);
        }
    }
}
