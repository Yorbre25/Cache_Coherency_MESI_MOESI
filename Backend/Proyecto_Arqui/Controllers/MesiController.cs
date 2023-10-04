using Microsoft.AspNetCore.Mvc;
using Proyecto_Arqui.Classes;
using System.Net;

namespace Proyecto_Arqui.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MesiController : ControllerBase
    {

        private readonly ILogger<MesiController> _logger;

        public MesiController(ILogger<MesiController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Read")]
        public OkResult Get(int address, int reg, int cpu_id)
        {
            MesiInterconnect.Instance.pass_inst("read", cpu_id,reg,address);
            return Ok();
        }

        [HttpPost("Write")]
        public OkResult Post(int address, int reg, int cpu_id)
        {
            MesiInterconnect.Instance.pass_inst("write", cpu_id, reg, address);
            return Ok();
        }

        [HttpPost("Increment")]
        public OkResult Post(int reg, int cpu_id)
        {
            MesiInterconnect.Instance.pass_inst("increment", cpu_id, reg, 0);
            return Ok();
        }
    }
}