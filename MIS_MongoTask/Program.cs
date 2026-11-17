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


        }
    }
}
