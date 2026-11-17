using MongoDataAccess.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDataAccess.DataAccess
{
    public class MongoDataAccess2
    {
        private const string connectionString = "mongodb://localhost:27017";
        private const string databaseName = "University";
        private const string StudentsCollection = "Students";
        private const string CoursesCollection = "Courses";

        // Connect to MongoDB
        private IMongoDatabase ConnectToMongo()
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            return database;
        }

        // Get Students collection
        private IMongoCollection<StudentModel> GetStudentsCollection()
        {
            var database = ConnectToMongo();

            return database.GetCollection<StudentModel>(StudentsCollection);
        }

        // Get Courses collection
        private IMongoCollection<CourseModel> GetCoursesCollection()
        {
            var database = ConnectToMongo();

            return database.GetCollection<CourseModel>(CoursesCollection);
        }

        // Q1 - Create collections and insert documents

        public async Task CreateCollectionsAndInsertData()
        {
            var database = ConnectToMongo();
            // Remove old collections if they already exist
            var collections = await database.ListCollectionNamesAsync();
            var collectionNames = await collections.ToListAsync();

            if (collectionNames.Contains(StudentsCollection))
            {
                await database.DropCollectionAsync(StudentsCollection);
            }

            if (collectionNames.Contains(CoursesCollection))
            {
                await database.DropCollectionAsync(CoursesCollection);
            }

            // Create Students collection
            await database.CreateCollectionAsync(StudentsCollection);

            // Create Courses collection
            await database.CreateCollectionAsync(CoursesCollection);

            var students = GetStudentsCollection();
            var courses = GetCoursesCollection();


            // Insert Students
            var studentList = new List<StudentModel>
            {
                new StudentModel
                {
                    Id = 1,
                    Name = "Ahmed",
                    Age = 20
                },

                new StudentModel
                {
                    Id = 2,
                    Name = "Mariam",
                    Age = 21
                },

                new StudentModel
                {
                    Id = 3,
                    Name = "Omar",
                    Age = 22
                }
            };

            await students.InsertManyAsync(studentList);

            // Insert Courses
            var courseList = new List<CourseModel>
            {
                new CourseModel
                {
                    Id = 1,
                    Name = "C#",
                    Instructor = "Ahmed"
                },

                new CourseModel
                {
                    Id = 2,
                    Name = "Database",
                    Instructor = "Mona"
                },

                new CourseModel
                {
                    Id = 3,
                    Name = "Web API",
                    Instructor = "Omar"
                }
            };

            await courses.InsertManyAsync(courseList);
            Console.WriteLine("Q1 completed.");
        }
        
        // Q2 - Delete one document from each collection
        public async Task DeleteDocuments()
        {
            var students = GetStudentsCollection();
            var courses = GetCoursesCollection();

            // Delete student with Id = 3
            await students.DeleteOneAsync(student => student.Id == 3);

            // Delete course with Id = 3
            await courses.DeleteOneAsync( course => course.Id == 3);
            Console.WriteLine("Q2 completed.");
        }
        // Q3 - Add Score array to at least two documents

        public async Task AddScores()
        {
            var students = GetStudentsCollection();
            var courses = GetCoursesCollection();

            // Students
            await students.UpdateOneAsync(student => student.Id == 1,Builders<StudentModel>
                .Update.Set(student => student.Score, new List<int> { 10, 20, 30 }));

            await students.UpdateOneAsync(student => student.Id == 2, Builders<StudentModel>
                .Update.Set(student => student.Score, new List<int> { 15, 25, 35 }) );

            // Courses
            await courses.UpdateOneAsync(course => course.Id == 1,Builders<CourseModel>
                .Update.Set(course => course.Score, new List<int> { 10, 20, 30 }));

            await courses.UpdateOneAsync(course => course.Id == 2,Builders<CourseModel>
                .Update .Set(course => course.Score, new List<int> { 15, 25, 35 }));
            Console.WriteLine("Q3 completed.");
        }

        // Q4 - Update Score depending on Id
        public async Task UpdateScoreById()
        {
            var students = GetStudentsCollection();
            var courses = GetCoursesCollection();

            // If Id = 1
            // Put 5 in the third position
            await students.UpdateOneAsync( student => student.Id == 1, Builders<StudentModel>.Update
                    .Set("Score.2", 5));

            await courses.UpdateOneAsync(course => course.Id == 1,Builders<CourseModel>.Update
                    .Set("Score.2", 5) );

            // If Id is not 1
            // Put 6 in the fourth position
            await students.UpdateOneAsync( student => student.Id == 2, Builders<StudentModel>.Update
                    .Set("Score.3", 6));

            await courses.UpdateOneAsync(course => course.Id == 2,Builders<CourseModel>.Update
                    .Set("Score.3", 6) );
            Console.WriteLine("Q4 completed.");
        }

        // Q5 - Multiply every Score element by 20
        public async Task MultiplyScoresBy20()
        {
            var database = ConnectToMongo();

            // Students as BsonDocument
            var students =database.GetCollection<BsonDocument>(StudentsCollection);

            // Courses as BsonDocument
            var courses = database.GetCollection<BsonDocument>(CoursesCollection);

            var studentPipeline = new[] { new BsonDocument("$set",new BsonDocument("Score",new BsonDocument( "$map", new BsonDocument
            {
                                    {
                                        "input",
                                        "$Score"
                                    },
                                    {
                                        "as",
                                        "score"
                                    },
                                    {
                                        "in",
                                        new BsonDocument(
                                            "$multiply",
                                            new BsonArray
                                            {
                                                "$$score",
                                                20
                                            }
                                        )
                                    }
                                }
                            )
                        )
                    )
                };


            var coursePipeline =
                new[]
                {
                    new BsonDocument(
                        "$set",
                        new BsonDocument(
                            "Score",
                            new BsonDocument(
                                "$map",
                                new BsonDocument
                                {
                                    {
                                        "input",
                                        "$Score"
                                    },
                                    {
                                        "as",
                                        "score"
                                    },
                                    {
                                        "in",
                                        new BsonDocument(
                                            "$multiply",
                                            new BsonArray
                                            {
                                                "$$score",
                                                20
                                            }
                                        )
                                    }
                                }
                            )
                        )
                    )
                };

            // Convert Student Pipeline to UpdateDefinition
            var studentUpdate = Builders<BsonDocument>.Update.Pipeline(
                PipelineDefinition<BsonDocument, BsonDocument>.Create(studentPipeline)
            );

            // Convert Course Pipeline to UpdateDefinition
            var courseUpdate = Builders<BsonDocument>.Update.Pipeline(
                PipelineDefinition<BsonDocument, BsonDocument>.Create(coursePipeline)
            );

            // Update all Students
            await students.UpdateManyAsync(
                new BsonDocument(),
                studentUpdate
            );

            // Update all Courses
            await courses.UpdateManyAsync(
                new BsonDocument(),
                courseUpdate
            );
            Console.WriteLine("Q5 completed.");
        }

        // Q6 - One Student has many Courses
        public async Task CreateRelationship()
        {
            var courses = GetCoursesCollection();


            var relatedCourses = new List<CourseModel>
            {
                new CourseModel
                {
                    Id = 4,
                    Name = "ASP.NET Core",
                    Instructor = "Mona",
                    StudentId = 1
                },

                new CourseModel
                {
                    Id = 5,
                    Name = "C# Advanced",
                    Instructor = "Ahmed",
                    StudentId = 1
                }
            };
            await courses.InsertManyAsync(relatedCourses);
            Console.WriteLine("Q6 completed.");
        }
        // Display Students

        public async Task ShowStudents()
        {
            var students = GetStudentsCollection();

            var result = await students
                .Find(_ => true)
                .ToListAsync();


            Console.WriteLine("\n========== STUDENTS ==========");


            foreach (var student in result)
            {
                Console.WriteLine(
                    $"Id: {student.Id}, " +
                    $"Name: {student.Name}, " +
                    $"Age: {student.Age}, " +
                    $"Score: [{string.Join(", ", student.Score)}]"
                );
            }
        }

        // Display Courses
        public async Task ShowCourses()
        {
            var courses = GetCoursesCollection();
            var result = await courses
                .Find(_ => true)
                .ToListAsync();

            Console.WriteLine("\n========== COURSES ==========");

            foreach (var course in result)
            {
                Console.WriteLine(
                    $"Id: {course.Id}, " +
                    $"Name: {course.Name}, " +
                    $"Instructor: {course.Instructor}, " +
                    $"StudentId: {course.StudentId}, " +
                    $"Score: [{string.Join(", ", course.Score)}]"
                );
            }
        }
    }
}
