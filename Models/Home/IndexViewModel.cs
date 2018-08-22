using System.Collections.Generic;
using vk_web_mock.Services;

namespace vk_web_mock.Models.Home
{

    public class IndexViewModel
    {
        public IEnumerable<Pod> Pods { get; set; }
    }

}