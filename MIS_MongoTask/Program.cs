using MongoDataAccess.DataAccess;
using MongoDataAccess.Models;
using MongoDB.Driver;

namespace MIS_MongoTask
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            MongoDataAccess2 db = new MongoDataAccess2();

            // Q1
            await db.CreateCollectionsAndInsertData();
            // Q2
            await db.DeleteDocuments();
            // Q3
            await db.AddScores();
            // Q4
            await db.UpdateScoreById();
            // Q5
            await db.MultiplyScoresBy20();
            // Q6
            await db.CreateRelationship();
            // Show final data
            await db.ShowStudents();

            await db.ShowCourses();
            Console.WriteLine("\nAll backend tasks completed.");

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("University");

            var students = database.GetCollection<StudentModel>("Students");
            var courses = database.GetCollection<CourseModel>("Courses");

            await students.InsertOneAsync(new StudentModel
            {
                Id = 10,
                Name = "Sara",
                Age = 23,
                Score = new List<int> { 50, 60, 70 }
            });

            await courses.InsertOneAsync(new CourseModel
            {
                Id = 10,
                Name = "Python",
                Instructor = "Mona",
                StudentId = 10,
                Score = new List<int> { 10, 20, 30 }
            });

            Console.WriteLine("Inserted successfully.");

            // Update
            await students.UpdateOneAsync(
                s => s.Id == 10,
                Builders<StudentModel>.Update.Set(s => s.Age, 24));

            // Delete
            await students.DeleteOneAsync(s => s.Id == 10);


        }
    }
}

