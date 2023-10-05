using Microsoft.AspNetCore.Mvc;
using Proyecto_Arqui.Classes.Moesi;
using System.Net;

namespace Proyecto_Arqui.Controllers
{
    [ApiController]
    [Route("Moesi")]
    public class MoesiController : ControllerBase
    {

        private readonly ILogger<MesiController> _logger;

        public MoesiController(ILogger<MesiController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Read")]
        public OkResult Get(int address, int reg, int cpu_id)
        {
            MoesiInterconnect.Instance.pass_inst("read", cpu_id,reg,address);
            return Ok();
        }

        [HttpPost("Write")]
        public OkResult Post(int address, int reg, int cpu_id)
        {
            MoesiInterconnect.Instance.pass_inst("write", cpu_id, reg, address);
            return Ok();
        }

        [HttpPost("Increment")]
        public OkResult Post(int reg, int cpu_id)
        {
            MoesiInterconnect.Instance.pass_inst("increment", cpu_id, reg, 0);
            return Ok();
        }
    }
}