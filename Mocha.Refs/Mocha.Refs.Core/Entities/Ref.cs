using Mocha.Common.Data;
using Mocha.Refs.Core.DataTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Refs.Core.Entities
{
    [Serializable]
    public class Ref: IAuditable
    {
        public long Id { get; set; }

        public RefKind Kind { get; set; }

        public string Uri { get; set; }

        public string Title { get; set; }

        public string Comment { get; set; }

        public bool IsRecommended { get; set; }

        public int DisplayOrder { get; set; }

        public long RefListId { get; set; }
        public virtual RefList RefList { get; set; }

        /// IAuditable
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public long CreatedUserId { get; set; }
        public long UpdatedUserId { get; set; }
    }
}
