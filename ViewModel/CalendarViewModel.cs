using System;
using System.Collections.Generic;

namespace pet_spa_system1.ViewModel
{
    public class CalendarViewModel
    {
        public List<CalendarEventViewModel> Events { get; set; } = new List<CalendarEventViewModel>();
        public List<StaffResourceViewModel> Resources { get; set; } = new List<StaffResourceViewModel>();
    }

    public class CalendarEventViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Status { get; set; }
        public string Customer { get; set; }
        public string Phone { get; set; }
        public List<string> PetNames { get; set; } = new List<string>();
        public List<string> Services { get; set; } = new List<string>();
        public string ResourceId { get; set; } // For staff assignment
        public string Color { get; set; } // Optional for status coloring
    }

    public class StaffResourceViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; } // Staff name
        public string BusinessHours { get; set; } // JSON string of business hours
        public string ImageUrl { get; set; } // Optional staff image
    }
}
