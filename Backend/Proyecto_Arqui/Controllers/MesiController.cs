using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Proyecto_Arqui.Classes;
using Proyecto_Arqui.Classes.Mesi;
using System.Net;
using System.Transactions;

namespace Proyecto_Arqui.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MesiController : ControllerBase
    {
        private static List<Transaction_tracker> response = new List<Transaction_tracker>();

        public static Transaction_tracker get_execution(int cpu_id)
        {
            if (response.Count < 3)
            {
                response.Add(new Transaction_tracker());
                response.Add(new Transaction_tracker());
                response.Add(new Transaction_tracker());
            }
            return response[cpu_id];
        }

        public static void set_execution(int cpu_id, Transaction_tracker exec)
        {
            if (response.Count < 3)
            {
                response.Add(new Transaction_tracker());
                response.Add(new Transaction_tracker());
                response.Add(new Transaction_tracker());
            }
            response[cpu_id] = exec;
        }

        private readonly ILogger<MesiController> _logger;

        public MesiController(ILogger<MesiController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Read")]
        public string Get(int address, int reg, int cpu_id)
        {
            MesiInterconnect.Instance.pass_inst("read", cpu_id,reg,address);
            Thread.Sleep(5000);
            
            return JsonConvert.SerializeObject(response[cpu_id]);
        }

        [HttpPost("Write")]
        public string Post(int address, int reg, int cpu_id)
        {
            
            MesiInterconnect.Instance.pass_inst("write", cpu_id, reg, address);
            return JsonConvert.SerializeObject(response[cpu_id]);
        }

        [HttpPost("Increment")]
        public OkResult Post(int reg, int cpu_id)
        {
            MesiInterconnect.Instance.pass_inst("increment", cpu_id, reg, 0);
            return Ok();
        }
    }
}