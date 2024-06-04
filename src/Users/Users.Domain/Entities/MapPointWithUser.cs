using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Users.Domain.Entities
{
    public class MapPointWithUser
    {
        public int PointId { get; set; }
        public string UserId { get; set; }

        public User User { get; set; }
        public MapPoint MapPoint { get; set; }
    }
}
