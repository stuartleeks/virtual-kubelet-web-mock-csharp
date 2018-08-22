using System;
using System.Collections.Generic;
using System.Linq;

namespace vk_web_mock.Services
{
    public class PodStore
    {
        private readonly List<Pod> _pods;

        public PodStore()
        {
            _pods = new List<Pod>();
        }

        public IEnumerable<Pod> GetPods()
        {
            return _pods;
        }

        public void AddPod(Pod pod)
        {
            _pods.Add(pod);
        }

        public Pod GetPod(string @namespace, string name)
        {
            // TODO consider using a Dictionary keyed on namesapce and pod name
            return _pods.FirstOrDefault(p=> p.Metadata.Namespace == @namespace && p.Metadata.Name == name);
        }
    }
}