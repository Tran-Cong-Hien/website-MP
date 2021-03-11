using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Products
{
    [Table("Products")]
    public class Products
    {
        [Key]
        public int Id { get; set; }
        public int IdCateGory { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double PriceSale { get; set; }
        public string Url { get; set; }
        public bool Active { get; set; }
        public bool Selling { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string MakeIn { get; set; }
        public string Allower { get; set; }
        public string NoteDetail { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdateBy { get; set; }
        public string images { get; set; }
    }

    public class ProductsFollowCateGory 
    {
        public List<Products> ListProduct { get; set; }
        public string NameCategory { get; set; }
    }

    public class ProductsDetail
    {
        public Products Detail { get; set; }
        public List<Products> ListProductLike { get; set; }

    }

    public class OrderCart
    {
        
        public List<ProductsCart> listSanpham { get; set; }
        public double TamTinh { get; set; }
        public double AllPayment { get; set; }

    }
    

    public class ProductsCart
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double PriceSale { get; set; }
        public string Url { get; set; }
        public int Quality { get; set; }
        public double AllPrice { get; set; }

    }
}

