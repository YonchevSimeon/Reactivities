namespace API.Controllers
{
    using System.Threading.Tasks;
    using Application.Photos;
    using Domain;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;

    public class PhotosController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<Photo>> Add([FromForm]Add.Command command)
        {
            return await this.Mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Delete(string id)
        {
            return await this.Mediator.Send(new Delete.Command { Id = id });
        }

        [HttpPost("{id}/setmain")]
        public async Task<ActionResult<Unit>> SetMain(string id)
        {
            return await this.Mediator.Send(new SetMain.Command { Id = id });
        }
    }
}