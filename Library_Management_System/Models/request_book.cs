//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Library_Management_System.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class request_book
    {
        public int ID { get; set; }
        public string BOOK_NAME { get; set; }
        public string AUTHER_NAME { get; set; }
        public string REQUEST_BY { get; set; }
        public string REMARK { get; set; }
        public Nullable<bool> IS_DELETED { get; set; }
        public Nullable<bool> IS_AVAILABLE { get; set; }
    }
}