using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocha.Common.Data
{
    public interface IAuditable
    {
        DateTime CreatedDate { get; set; }
        DateTime UpdatedDate { get; set; }
        long CreatedUserId { get; set; }
        long UpdatedUserId { get; set; }
    }
}
