using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MON.Shared.Interfaces;
using System.Threading.Tasks;

namespace MON.Services.Hubs
{
    [Authorize]
    public class StudentHub : Hub<IStudentHub>
    {
        public async Task JoinStudentGroup(int personId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, personId.ToString());
        }

        public async Task LeaveStudentGroup(int personId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, personId.ToString());
        }
    }
}
