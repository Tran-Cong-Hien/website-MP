using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Home
{    
    public class HomeModel
    {
        public List<Data.Products.Products> ListNewsProducts{ get; set; }
        public List<Data.News.News> ListNews{ get; set; }
        public List<Data.Products.Products> ListSellProducts{ get; set; }
    }
}
