//------------------------------------------------------------------------------
// <auto-generated>
//    這個程式碼是由範本產生。
//
//    對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//    如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace PWA_TEST.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PWA_Table
    {
        public int ID { get; set; }
        public string endpoint { get; set; }
        public string p256dh { get; set; }
        public string auth { get; set; }
        public Nullable<bool> Isdelete { get; set; }
        public Nullable<int> Cancel { get; set; }
        public Nullable<System.DateTime> Create { get; set; }
        public Nullable<System.DateTime> Update { get; set; }
    }
}