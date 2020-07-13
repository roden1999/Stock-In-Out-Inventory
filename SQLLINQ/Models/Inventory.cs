using System;
using System.Collections.Generic;

namespace SQLLINQ.Models
{
    public partial class Inventory
    {
        public long PackageId { get; set; }
        public string Image { get; set; }
        public string PackageName { get; set; }
        public int Category { get; set; }
        public string Description { get; set; }
        public int StockIn { get; set; }
        public int StockOut { get; set; }
        public DateTime Date { get; set; }
        public bool IsDeleted { get; set; }
    }
}
