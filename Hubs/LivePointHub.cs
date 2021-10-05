using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VueExample.ViewModels;
using VueExample.Providers.ChipVerification.Abstract;
using VueExample.Services.Vertx.Abstract;

namespace VueExample.Hubs
{
    public class LivePointHub : Hub
    {
        private readonly ILivePointService _livePointService;
        public LivePointHub(ILivePointService livePointService)
        {
            _livePointService = livePointService;            
        }
        public async Task GetLastValues()
        {
            await Clients.Caller.SendAsync("lastValues", await _livePointService.GetNLastPoint(1000));
        }
    }
}