using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using vk_web_mock.Models;
using vk_web_mock.Models.Home;
using vk_web_mock.Services;

namespace vk_web_mock.Controllers
{
    public class HomeController : Controller
    {
        private readonly PodStore _podStore;

        public HomeController(PodStore podStore)
        {
            _podStore = podStore;
        }
        [HttpGet("")]
        public IActionResult Index()
        {
            var model = new IndexViewModel
            {
                Pods = _podStore.GetPods()
            };
            return View(model);
        }

        [HttpGet("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
