using System;
using System.Collections.Generic;

namespace Mocha.Refs.Data.Models
{
    public partial class Ref
    {
        public long Id { get; set; }
        public byte Kind { get; set; }
        public string Uri { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public bool IsRecommended { get; set; }
        public int DisplayOrder { get; set; }
        public long RefListId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public System.DateTime UpdatedDate { get; set; }
        public long CreatedUserId { get; set; }
        public long UpdatedUserId { get; set; }
        public virtual RefList RefList { get; set; }
    }
}
