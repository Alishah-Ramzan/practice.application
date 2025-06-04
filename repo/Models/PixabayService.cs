using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.Models
{
    public class PixabayResponse
    {
        public PixabayHit[] Hits { get; set; } = Array.Empty<PixabayHit>();
    }

    public class PixabayHit
    {
        public string Tags { get; set; } = "";
        public string WebformatURL { get; set; } = "";
    }
}
