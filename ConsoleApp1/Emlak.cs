using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
   public class Emlak
    {
        public int EmlakId { get; set; }
        public string ElanId { get; set; }
        public string ElanUrl { get; set; }
        public string Type { get; set; }
        public string PapperWork { get; set; }
        public string Price { get; set; }
        public string Location { get; set; }
        public string Otaqlar { get; set; }
        public string Kvadrat { get; set; }
        public string Date { get; set; }
        public string Floor { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string PhoneNumber { get; set; }
    }

}
