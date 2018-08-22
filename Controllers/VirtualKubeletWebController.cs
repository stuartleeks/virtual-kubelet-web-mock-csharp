using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using vk_web_mock.Models;

namespace vk_web_mock.Controllers
{
    [ApiController]
    public class VirtualKubeletWebController : Controller
    {
        private readonly ILogger _logger;
        public VirtualKubeletWebController(ILogger<VirtualKubeletWebController> logger)
        {
            _logger = logger;
        }
        [HttpGet("capacity")]
        public IActionResult GetCapacity()
        {
            return Json(new {
                cpu = "20",
                memory = "100Gi",
                pods ="20"
            });
        }

        [HttpGet("{*unmatched}")]
        public IActionResult Catchall(string unmatched)
        {
            _logger.LogWarning($"Unmatched: {unmatched}");
            return NotFound();
        }
    }
}
