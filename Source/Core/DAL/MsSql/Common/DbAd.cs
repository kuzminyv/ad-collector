//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Core.DAL.MsSql.Common
{
    using System;
    using System.Collections.Generic;
    
    public partial class DbAd
    {
        public DbAd()
        {
            this.AdLinks = new HashSet<DbAdLink>();
            this.AdLinks1 = new HashSet<DbAdLink>();
            this.AdHistoryItems = new HashSet<DbAdHistoryItem>();
            this.Metadatas = new HashSet<DbMetadata>();
            this.AdImages = new HashSet<DbAdImage>();
        }
    
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public System.DateTime PublishDate { get; set; }
        public System.DateTime CollectDate { get; set; }
        public double Price { get; set; }
        public string IdOnWebSite { get; set; }
        public bool IsSuspicious { get; set; }
        public string ConnectorId { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public string Url { get; set; }
    
        public virtual ICollection<DbAdLink> AdLinks { get; set; }
        public virtual ICollection<DbAdLink> AdLinks1 { get; set; }
        public virtual ICollection<DbAdHistoryItem> AdHistoryItems { get; set; }
        public virtual ICollection<DbMetadata> Metadatas { get; set; }
        public virtual ICollection<DbAdImage> AdImages { get; set; }
    }
}
