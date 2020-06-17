using System;
using System.Collections.Generic;

namespace TesteExcel.Models
{
    public partial class CreateActivityRequests
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public int Points { get; set; }
        public int Period { get; set; }
        public int SelectedVerificationType { get; set; }
    }
}
