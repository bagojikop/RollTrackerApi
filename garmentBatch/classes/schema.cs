using garmentBatch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace garmentBatch.classes
{
    public class result
    {
        public byte status { get; set; }
        public dynamic data { get; set; }
        public string message { get; set; }
    }
    public class batch
    {
        public string finYear { get; set; }
        public int BatchNo { get; set; }
        public string loomNo { get; set; }
    }
    public class batchItems
    {
        public string finYear { get; set; }
        public int BatchNo { get; set; }
        public string loomNo { get; set; }
        public DateTime BatchCreated { get; set; }
        public string inspectedby { get; set; }
        public string verifiedby { get; set; }
        public DateTime? inspectedDate { get; set; }
        public string product { get; set; }
        public string QcCode { get; set; }
        public string shade { get; set; }
        public string godown { get; set; }
        public string stackNo { get; set; }
        public decimal? mtrs { get; set; }
        public decimal? weight { get; set; }
        public string width { get; set; }
        public int? slipNo { get; set; }
        public DateTime? slipDate { get; set; }
        public string custname { get; set; }
        public string location { get; set; }
        public int status { get; set; }
        public string statusname { get; set; }
        public int selected { get; set; }
        public bool rejected { get; set; }
        public string rejectedResean { get; set; }
        public string rejectedName { get; set; }
        public string narration { get; set; }
        public string grade { get; set; }
    }


    public class outwarddetails
    {
        public long VchId { get; set; }
        public int VchNo { get; set; }

        public DateTime VchDate { get; set; }

        public int req_id {  get; set; }
        public int gwn_code { get; set; }
        public string palletId { get; set; }
        public string vehicleNo { get; set; }
        public string finyear { get; set; }

        public string loomNo { get; set; }

        public int Mtrs { get; set; }

        public int bthNo { get; set; }

        public string quality { get; set; }
        public string productName { get; set; }

        public string CreatedUser { get; set; }

        public DateTime? CreatedDate { get; set; }

       


    }


    public class QueryStringParameters
    {
        const int maxPageSize = 25;
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = (value > maxPageSize) ? maxPageSize : value;
            }
        }
        public List<values> keys { get; set; } = new List<values>();
    }

    public class packinglist
    {
        public int slipid { get; set; }
        public string finYear { get; set; }
        public int slipNo { get; set; }
        public DateTime slipDate { get; set; }
        public string custname { get; set; }
        public string location { get; set; }
        public string vehicleNo { get; set; }
        public string product { get; set; }
        public string QcCode { get; set; }
        public decimal? mtrs { get; set; }
        public decimal? weight { get; set; }



    }
}