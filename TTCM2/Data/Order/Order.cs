using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Order
{
    [Table("Od_Order")]
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public double IntoMoney { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class OrderModel
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public double IntoMoney { get; set; }
        public DateTime CreateDate { get; set; }
        public List<Data.OrderDetail.OrderDetail> ListOrderDetail { get; set; }
    }

    public class OrderCartSuccess
    {
        public List<Data.OrderDetail.OrderDetail> listSanpham { get; set; }
        public Order DonHang { get; set; }

    }
}
