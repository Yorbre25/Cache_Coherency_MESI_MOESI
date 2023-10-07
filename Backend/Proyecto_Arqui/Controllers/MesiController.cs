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
        private readonly ILogger<MesiController> _logger;

        public MesiController(ILogger<MesiController> logger)
        {
            _logger = logger;
        }
        
        
        [HttpGet("run_all")]
        public string Get()
        {
            var msg = new front_end_data(MesiInterconnect.Instance);
            var temp = MesiInterconnect.Instance.run_all();
            msg.update_data(MesiInterconnect.Instance, temp);
            Create_Report.create_file(msg, temp);
            return JsonConvert.SerializeObject(msg, Formatting.Indented);
        }

        [HttpGet("run")]
        public string Get1(int CPU_1, int CPU_2, int CPU_3)
        {
            bool cpu1 = false;
            bool cpu2 = false;
            bool cpu3 = false;
            if (CPU_1 != 0)
            {
                cpu1 = true;
            }
            if (CPU_2 != 0)
            {
                cpu2 = true;
            }
            if (CPU_3 != 0)
            {
                cpu3 = true;
            }

            var msg = new front_end_data(MesiInterconnect.Instance);
            var temp = MesiInterconnect.Instance.run_all(cpu1, cpu2, cpu3);
            msg.update_data(MesiInterconnect.Instance, temp);

            return JsonConvert.SerializeObject(msg, Formatting.Indented);
        }

        [HttpGet("step")]
        public string Get(int CPU_1,int CPU_2, int CPU_3)
        {
            bool cpu1 = false;
            bool cpu2 = false;
            bool cpu3 = false;
            if (CPU_1 != 0)
            {
                cpu1 = true;
            }
            if (CPU_2 != 0)
            {
                cpu2 = true;
            }
            if (CPU_3 != 0)
            {
                cpu3 = true;
            }

            var msg = new front_end_data(MesiInterconnect.Instance);
            var temp = MesiInterconnect.Instance.run(cpu1, cpu2, cpu3);
            msg.update_data(MesiInterconnect.Instance, temp);
            //Create_Report.create_file(msg, temp);
            return JsonConvert.SerializeObject(msg, Formatting.Indented);
        }
    }
}