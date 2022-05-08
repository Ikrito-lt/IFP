﻿using System.Collections.Generic;

namespace IFP.Modules.Supplier.KotrynaGroup.Modules
{
    class KGProductInfo {
        public string axapta_id { set; get; }
        public string kpn { set; get; }
        public string coo { set; get; }

        public string titleLT { set; get; }
        public string titleLV { set; get; }
        public string titleEE { set; get; }
        public string titleRU { set; get; }
        public string vendorType { set; get; }
        public string brand { set; get; }

        public Dictionary<string, string> properties = new();
        public List<string> images = new();
    }
}
