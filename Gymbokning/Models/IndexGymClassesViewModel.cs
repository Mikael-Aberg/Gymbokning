using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gymbokning.Models
{
    public class IndexGymClassesViewModel
    {
        public string Checked { get; set; }
        public ICollection<ClassAttendedViewModel> GymClasses { get; set; }
    }
}