using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarenisoft.DB.Mysql
{
    public class Pagination
    {
        public int page { get; set; }

        public int pageSize { get; set; }


        public int Offset { get {
                return (page * pageSize)- pageSize;
            }
        }

        public int collectionSize { get; set; }
    }
}
