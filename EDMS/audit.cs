//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EDMS
{
    using System;
    using System.Collections.Generic;
    
    public partial class audit
    {
        public int Id { get; set; }
        public int personId { get; set; }
        public System.DateTime datetime { get; set; }
        public int activityId { get; set; }
        public string auditObject { get; set; }
    
        public virtual activity activity { get; set; }
        public virtual person person { get; set; }
    }
}