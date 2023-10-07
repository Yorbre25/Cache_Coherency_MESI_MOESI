using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto_Arqui.Classes;
using Proyecto_Arqui.Classes.Moesi;
using System.Net;

namespace Proyecto_Arqui.Controllers
{
    [ApiController]
    [Route("Moesi")]
    public class MoesiController : ControllerBase
    {

        private readonly ILogger<MoesiController> _logger;

        public MoesiController(ILogger<MoesiController> logger)
        {
            _logger = logger;
        }

        [HttpGet("run_all")]
        public string Get()
        {
            var msg = new front_end_data(MoesiInterconnect.Instance);
            var temp = MoesiInterconnect.Instance.run_all();
            msg.update_data(MoesiInterconnect.Instance, temp);
            return JsonConvert.SerializeObject(msg, Formatting.Indented);
        }
    }
}