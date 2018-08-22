using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vk_web_mock.Models;

namespace vk_web_mock.Controllers
{
    [ApiController]
    public class VirtualKubeletWebController : Controller
    {
        [HttpGet("capacity")]
        public IActionResult GetCapacity()
        {
            return Json(new {
                cpu = "20",
                memory = "100Gi",
                pods ="20"
            });
        }
    }
}
