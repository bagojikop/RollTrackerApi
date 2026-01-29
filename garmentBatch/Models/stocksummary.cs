using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace garmentBatch.Models
{
    public partial class stocksummary
    {


        public class quality1
        {
            public string Qc_Code { get; set; }
            public int Mtrs { get; set; }

            public prod1 prod1 { get; set; }
        }

        public class prod1
        {
            public int prodCode { get; set; }
            public string prodName { get; set; }
            public int Mtrs { get; set; }
            public string rollNo { get; set; }



            public inward1 inward1 { get; set; }

        }

        public class inward1
        {
            public string finyear { get; set; }
            public string loomNo { get; set; }
            public int batchNo { get; set; }
            public int totalMtrs { get; set; }

        }
    }
   
}