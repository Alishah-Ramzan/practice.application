using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//add jsonproperty here
namespace Repo.Models
{
    public class Image
    {
        public int Id { get; set; }                // Primary Key
        public string Query { get; set; } = "";    // What the user searched
        public string Tag { get; set; } = "";      // First tag returned
        public string Url { get; set; } = "";      // First image URL returned
    }
}
