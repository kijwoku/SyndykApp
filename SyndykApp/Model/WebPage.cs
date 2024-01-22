using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyndykApp.Model
{
    public class WebPage
    {
        public string URL { get; set; }
        public List<Advertisement> Advertisements { get; set; }
    }
}
