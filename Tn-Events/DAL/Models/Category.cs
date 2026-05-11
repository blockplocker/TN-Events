using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class Category
    {
        public required int Id { get; set; }
        public required string Name { get; set; }

        public ICollection<Event> Events { get; set; }
    }
}
