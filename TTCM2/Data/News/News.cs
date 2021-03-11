using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.News
{
    [Table("News")]
    public class News
    {
        
      
       [Key]
            public int Id { get; set; }
            public int IdCateGory { get; set; }
            public string Name { get; set; }
            public string Url { get; set; }
            public bool Active { get; set; }
            public string Note { get; set; }
            public string NoteDetail { get; set; }
            public DateTime CreateDate { get; set; }
            public string CreateBy { get; set; }
            public DateTime UpdateDate { get; set; }
            public string UpdateBy { get; set; }
        
    }


    public class NewsModel
    {
         public List <News> ListHomeNews { get; set; }
        public List<News> ListOtherNews { get; set; }
        
    }
}
