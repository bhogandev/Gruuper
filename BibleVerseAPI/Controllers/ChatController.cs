using BibleVerseAPI.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibleVerseAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

       [Route("Send")]
       [HttpPost]
       public IActionResult SendRequest([FromBody] BibleVerse.DTO.Messages msg, string conID)
        {
            _hubContext.Clients.Client(conID).SendAsync("RecieveOne", msg.Username, msg.Body);
            return Ok();
        }
    }
}
