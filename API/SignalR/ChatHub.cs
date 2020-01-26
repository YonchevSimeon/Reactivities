namespace API.SignalR
{
    using MediatR;
    using Microsoft.AspNetCore.SignalR;
    using Application.Comments;
    using Application.Comments.DTOs;
    using System.Threading.Tasks;
    using System.Linq;
    using System.Security.Claims;

    public class ChatHub : Hub
    {
        private readonly IMediator mediator;

        public ChatHub(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task SendComment(Create.Command command)
        {
            string username = this.GetUsername();

            command.Username = username;

            CommentDto comment = await this.mediator.Send(command);

            await this.Clients.Group(command.ActivityId.ToString()).SendAsync("ReceiveComment", comment);
        }

        public async Task AddToGroup(string groupName)
        {
            await this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
        
            await this.Clients.Group(groupName).SendAsync("Send", $"{this.GetUsername()} has joined the group");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await this.Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupName);
        
            await this.Clients.Group(groupName).SendAsync("Send", $"{this.GetUsername()} has left the group");
        }

        private string GetUsername()
        {
            return this.Context.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}