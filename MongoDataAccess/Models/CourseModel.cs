using System;
using System.Collections.Generic;
using System.Text;

namespace MongoDataAccess.Models
{
    public class CourseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Instructor { get; set; }

        public List<int> Score { get; set; } = new List<int>();

        public int? StudentId { get; set; } // One Student → Many Courses
    }
}
