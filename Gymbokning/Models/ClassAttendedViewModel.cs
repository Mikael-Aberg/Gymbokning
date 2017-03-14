using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gymbokning.Models
{
    public class ClassAttendedViewModel
    {
        public bool Attended { get; set; }
        public bool IsOldClass { get; set; }
        public GymClass GymClass { get; set; }
    }
}