using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Messenger
{
    public class MessengerResult
    {
        public bool success { get; set; }
        public string messenger { get; set; }
        public int OrderId { get; set; }

    }

    public class Cart
    {
        public int ProductId { get; set; }
        public int Number { get; set; }
    }
}
