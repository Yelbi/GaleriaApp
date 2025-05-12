using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GaleriaApp.Models
{
    public class Album
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string CoverImagePath { get; set; }
        public DateTime DateCreated { get; set; }
        public List<string> MediaItemIds { get; set; } = new List<string>();
    }
}