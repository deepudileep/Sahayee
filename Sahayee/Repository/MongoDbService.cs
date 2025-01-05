using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Sahayee.Models.DB;
using Sahayee.Models.ViewModel;
using System.Collections.Generic;
using System.Linq;
using static Sahayee.Controllers.JobController;

namespace Sahayee.Repository
{
    public class MongoDbService<T> where T : class
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDbService(IMongoClient mongoClient, IConfiguration configuration)
        {
            var databaseName = configuration.GetValue<string>("MongoDb:DatabaseName");
            var database = mongoClient.GetDatabase(databaseName);
            _collection = database.GetCollection<T>(typeof(T).Name);  // Use the type's name as the collection name
        }

        public void Insert(T entity)
        {
            _collection.InsertOne(entity);
        }

        public T GetById(ObjectId id)
        {
            return _collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefault();
        }

        public IEnumerable<T> Get()
        {
            return _collection.Find(job => true).ToList();
        }

        public List<T> ApplyFilters(Dictionary<string, string> filterCriteria)
        {
            var filterBuilder = Builders<T>.Filter;
            var filters = new List<FilterDefinition<T>>();

            foreach (var criteria in filterCriteria)
            {
                if (!string.IsNullOrWhiteSpace(criteria.Value) && criteria.Value != "all")
                {
                    // Dynamically build filters based on the property name and value
                    filters.Add(filterBuilder.Eq(criteria.Key, criteria.Value));
                }
            }

            var combinedFilter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;

            return _collection.Find(combinedFilter).ToList();
        }

        public List<T> ApplyFiltersObject(Dictionary<string, ObjectId> filterCriteria)
        {
            var filterBuilder = Builders<T>.Filter;
            var filters = new List<FilterDefinition<T>>();

            foreach (var criteria in filterCriteria)
            {
                // Dynamically build filters based on the property name and value
                filters.Add(filterBuilder.Eq(criteria.Key, criteria.Value));
            }

            var combinedFilter = filters.Count > 0 ? filterBuilder.And(filters) : filterBuilder.Empty;

            return _collection.Find(combinedFilter).ToList();
        }

        // Delete by Id
        public void DeleteById(ObjectId id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            _collection.DeleteOne(filter);
        }

        // Delete based on a filter
        public void DeleteByFilter(FilterDefinition<T> filter)
        {
            _collection.DeleteMany(filter);
        }

        // Update by Id
        public void UpdateById(ObjectId id, T updatedEntity)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            _collection.ReplaceOne(filter, updatedEntity);
        }

        // Update with a filter (partial update)
        public void UpdateByFilter(FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            _collection.UpdateMany(filter, update);
        }
        public void UpdateLastModified(FilterDefinition<T> filter)
        {
            // Create the update definition to set the LastModified field to the current UTC time
            var update = Builders<T>.Update.Set("LastModified", DateTime.Now);

            // Apply the update to all matching documents
            _collection.UpdateMany(filter, update);
        }
        public async Task<List<CountType>> CountDistinctValuesAsync(string fieldName)
        {
            // Aggregation pipeline to count distinct values of the specified field
            var fieldCount = await _collection.Aggregate()
                .Group(new BsonDocument
                {
            { "_id", $"${fieldName}" }  // Group by the specified field
                })
                .Project(new BsonDocument
                {
            { "Field", "$_id" },
            { "Count", new BsonDocument("$sum", 1) }  // Count the occurrences of each distinct value
                })
                .ToListAsync();

            // Return the aggregated count results
            return fieldCount.Select(x => new CountType
            {
                Type = x["Field"]?.ToString(),
                Count = x["Count"].ToInt32()
            }).ToList();
        }

        public async Task<List<JobApplicationWithDetails>> GetJobsAppliedByJobAsync(string jobId)
        {
            var jobApplicationCollection = _collection.Database.GetCollection<JobApplication>("JobApplication");

            var pipeline = new List<BsonDocument>
    {
        new BsonDocument
        {
            { "$match", new BsonDocument { { "JobId", jobId } } }
        },
        new BsonDocument
        {
            { "$addFields", new BsonDocument
                {
                    { "JobId", new BsonDocument { { "$toObjectId", "$JobId" } } },
                    { "UserId", new BsonDocument { { "$toObjectId", "$UserId" } } } // Ensure matching types
                }
            }
        },
        new BsonDocument
        {
            { "$lookup", new BsonDocument
                {
                    { "from", "Jobs" },
                    { "localField", "JobId" },
                    { "foreignField", "_id" },
                    { "as", "jobDetails" }
                }
            }
        },
        new BsonDocument
        {
            { "$unwind", "$jobDetails" }
        },
        new BsonDocument
        {
            { "$lookup", new BsonDocument
                {
                    { "from", "User" },
                    { "localField", "UserId" },
                    { "foreignField", "_id" },
                    { "as", "userDetails" }
                }
            }
        },
        new BsonDocument
        {
            { "$unwind", "$userDetails" }
        },
        new BsonDocument
        {
            { "$project", new BsonDocument
                {
                    { "_id", 1 },
                    { "UserId", 1 },
                    { "JobId", 1 },
                    { "Status", 1 },
                    { "UserMessage", 1 },
                    { "AppliedOn", 1 },
                    { "FollowUpDate", 1 },
                    { "FollowUpBy", 1 },
                    { "FollowUpHistory", 1 },
                    { "JobDetails", new BsonDocument
                        {
                            { "_id", "$jobDetails._id" },
                            { "JobTitle", "$jobDetails.JobTitle" },
                            { "Department", "$jobDetails.Department" },
                            { "Institution", "$jobDetails.Institution" },
                            { "Location", "$jobDetails.Location" },
                            { "ContactEmail", "$jobDetails.ContactEmail" },
                            { "Description", "$jobDetails.Description" }
                        }
                    },
                    { "UserDetails", new BsonDocument
                        {
                            { "_id", "$userDetails._id" },
                            { "FirstName", "$userDetails.FirstName" },
                            { "LastName", "$userDetails.LastName" },
                            { "Email", "$userDetails.Email" },
                            { "PhoneNumber", "$userDetails.PhoneNumber" },
                            { "Location", "$userDetails.Location" },
                            { "DOB", "$userDetails.DOB" },
                            { "Gender", "$userDetails.Gender" },
                            { "UserType", "$userDetails.UserType" }
                        }
                    }
                }
            }
        }
    };

            var result = await jobApplicationCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();

            if (result == null || !result.Any())
            {
                Console.WriteLine("No matching job applications found.");
                return new List<JobApplicationWithDetails>();
            }

            Console.WriteLine($"Found {result.Count} job applications.");
            return result.Select(x => BsonSerializer.Deserialize<JobApplicationWithDetails>(x)).ToList();
        }

        public List<User> GetCandidates(string name = null, string occupation = null)
        {
            var _usersCollection = _collection.Database.GetCollection<User>("User");

            var filterBuilder = Builders<User>.Filter;
            var filter = filterBuilder.Eq("UserType.Type", "User");

            if (!string.IsNullOrEmpty(name))
            {
                filter &= filterBuilder.Regex("FirstName", new BsonRegularExpression(name, "i")) |
                          filterBuilder.Regex("LastName", new BsonRegularExpression(name, "i"));
            }

            if (!string.IsNullOrEmpty(occupation))
            {
                filter &= filterBuilder.Regex("Occupation", new BsonRegularExpression(occupation, "i"));
            }

            return _usersCollection.Find(filter).ToList();
        }
    public UserStatistics GetUserStatistics(string userId)
    {
        var _jobApplications = _collection.Database.GetCollection<JobApplication>("JobApplication");
        var _courseApplications = _collection.Database.GetCollection<CourseApplication>("CourseApplication");
        var _queriesCollection = _collection.Database.GetCollection<Queries>("Queries");
        // Job Applications Count
        var jobCount = _jobApplications.CountDocuments(new BsonDocument("UserId", userId));

        // Course Applications Count
        var courseCount = _courseApplications.CountDocuments(new BsonDocument("UserId", userId));

        // Queries Count
        var queryCount = _queriesCollection.CountDocuments(new BsonDocument("UserId", userId));

        return new UserStatistics
        {
            UserId = userId,
            JobCount = (int)jobCount,
            CourseCount = (int)courseCount,
            QueryCount = (int)queryCount
        };
    }

    public async Task<List<JobApplicationWithDetails>> GetJobsAppliedByUserAsync(string userId)
    {
        var jobApplicationCollection = _collection.Database.GetCollection<JobApplication>("JobApplication");

        var pipeline = new List<BsonDocument>
    {
        new BsonDocument
        {
            { "$match", new BsonDocument { { "UserId", userId } } }
        },
        new BsonDocument
        {
            { "$addFields", new BsonDocument
                {
                    { "JobId", new BsonDocument { { "$toObjectId", "$JobId" } } },
                    { "UserId", new BsonDocument { { "$toObjectId", "$UserId" } } } // Ensure matching types
                }
            }
        },
        new BsonDocument
        {
            { "$lookup", new BsonDocument
                {
                    { "from", "Jobs" },
                    { "localField", "JobId" },
                    { "foreignField", "_id" },
                    { "as", "jobDetails" }
                }
            }
        },
        new BsonDocument
        {
            { "$unwind", "$jobDetails" }
        },
        new BsonDocument
        {
            { "$lookup", new BsonDocument
                {
                    { "from", "User" },
                    { "localField", "UserId" },
                    { "foreignField", "_id" },
                    { "as", "userDetails" }
                }
            }
        },
        new BsonDocument
        {
            { "$unwind", "$userDetails" }
        },
        new BsonDocument
        {
            { "$project", new BsonDocument
                {
                    { "_id", 1 },
                    { "UserId", 1 },
                    { "JobId", 1 },
                    { "Status", 1 },
                    { "UserMessage", 1 },
                    { "AppliedOn", 1 },
                    { "FollowUpDate", 1 },
                    { "FollowUpBy", 1 },
                    { "FollowUpHistory", 1 },
                    { "JobDetails", new BsonDocument
                        {
                            { "_id", "$jobDetails._id" },
                            { "JobTitle", "$jobDetails.JobTitle" },
                            { "Department", "$jobDetails.Department" },
                            { "Institution", "$jobDetails.Institution" },
                            { "Location", "$jobDetails.Location" },
                            { "ContactEmail", "$jobDetails.ContactEmail" },
                            { "Description", "$jobDetails.Description" }
                        }
                    },
                    { "UserDetails", new BsonDocument
                        {
                            { "_id", "$userDetails._id" },
                            { "FirstName", "$userDetails.FirstName" },
                            { "LastName", "$userDetails.LastName" },
                            { "Email", "$userDetails.Email" },
                            { "PhoneNumber", "$userDetails.PhoneNumber" },
                            { "Location", "$userDetails.Location" },
                            { "DOB", "$userDetails.DOB" },
                            { "Gender", "$userDetails.Gender" },
                            { "UserType", "$userDetails.UserType" }
                        }
                    }
                }
            }
        }
    };

        var result = await jobApplicationCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();

        if (result == null || !result.Any())
        {
            Console.WriteLine("No matching job applications found.");
            return new List<JobApplicationWithDetails>();
        }

        Console.WriteLine($"Found {result.Count} job applications.");
        return result.Select(x => BsonSerializer.Deserialize<JobApplicationWithDetails>(x)).ToList();
    }


    public async Task<List<JobApplication>> GetJobApplicationsByJobIdAsync(string jobId)
    {
        var jobApplicationCollection = _collection.Database.GetCollection<JobApplication>("JobApplication");
        // Query to find all JobApplications with the specified JobId
        var filter = Builders<JobApplication>.Filter.Eq(app => app.JobId, jobId);
        return await jobApplicationCollection.Find(filter).ToListAsync();
    }
    public async Task<List<CourseApplicationWithDetails>> GetCoursesAppliedByUserAsync(string userId)
    {
        var courseApplicationCollection = _collection.Database.GetCollection<CourseApplication>("CourseApplication");

        // Convert userId to ObjectId if needed
        var pipeline = new List<BsonDocument>
    {
        // Match stage to filter by UserId
        new BsonDocument
        {
            { "$match", new BsonDocument { { "UserId", userId } } }
        },
        // Convert CourseId to ObjectId if necessary
        new BsonDocument
        {
            { "$addFields", new BsonDocument
                {
                    { "CourseId", new BsonDocument { { "$toObjectId", "$CourseId" } } },
                    { "UserId", new BsonDocument { { "$toObjectId", "$UserId" } } } // Ensure matching types
                }
            }
        },
        // Lookup stage to join with Course collection
        new BsonDocument
        {
            { "$lookup", new BsonDocument
                {
                    { "from", "Course" },
                    { "localField", "CourseId" },
                    { "foreignField", "_id" },
                    { "as", "courseDetails" }
                }
            }
        },
        // Unwind to flatten the joined data
        new BsonDocument
        {
            { "$unwind", "$courseDetails" }
        },

        new BsonDocument
        {
            { "$lookup", new BsonDocument
                {
                    { "from", "User" },
                    { "localField", "UserId" },
                    { "foreignField", "_id" },
                    { "as", "userDetails" }
                }
            }
        },
        new BsonDocument
        {
            { "$unwind", "$userDetails" }
        },
        // Project to include specific fields
        new BsonDocument
        {
            { "$project", new BsonDocument
                {
                    { "_id", 1 },
                    { "UserId", 1 },
                    { "CourseId", 1 },
                    { "Status", 1 },
                    { "UserMessage", 1 },
                    { "AppliedOn", 1 },
                    { "FollowUpDate", 1 },
                    { "FollowUpBy", 1 },
                    { "FollowUpHistory", 1 },
                    { "CourseDetails", new BsonDocument
                        {
                            { "_id", "$courseDetails._id" },
                            { "Name", "$courseDetails.Name" },
                            { "Category", "$courseDetails.Category" },
                            { "Institution", "$courseDetails.Institution" },
                            { "Location", "$courseDetails.Location" },
                            { "Duration", "$courseDetails.Duration" },
                            { "Summary", "$courseDetails.Summary" },
                            { "Trainer", "$courseDetails.Trainer" },
                            { "StartDate", "$courseDetails.StartDate" },
                        }
                    },
                    { "UserDetails", new BsonDocument
                        {
                            { "_id", "$userDetails._id" },
                            { "FirstName", "$userDetails.FirstName" },
                            { "LastName", "$userDetails.LastName" },
                            { "Email", "$userDetails.Email" },
                            { "PhoneNumber", "$userDetails.PhoneNumber" },
                            { "Location", "$userDetails.Location" },
                            { "DOB", "$userDetails.DOB" },
                            { "Gender", "$userDetails.Gender" },
                            { "UserType", "$userDetails.UserType" }
                        }
                    }
                }
            }
        }
    };

        // Execute the pipeline
        var result = await courseApplicationCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();

        if (result == null || !result.Any())
        {
            return new List<CourseApplicationWithDetails>();
        }

        // Deserialize the result to the target model
        var courseApplicationsList = result.Select(x =>
            BsonSerializer.Deserialize<CourseApplicationWithDetails>(x)).ToList();

        return courseApplicationsList;
    }

    public async Task<List<CourseApplicationWithDetails>> GetCoursesAppliedByCourseAsync(string cId)
    {
        var courseApplicationCollection = _collection.Database.GetCollection<CourseApplication>("CourseApplication");

        // Convert userId to ObjectId if needed
        var pipeline = new List<BsonDocument>
    {
        // Match stage to filter by UserId
        new BsonDocument
        {
            { "$match", new BsonDocument { { "CourseId", cId } } }
        },
        // Convert CourseId to ObjectId if necessary
        new BsonDocument
        {
            { "$addFields", new BsonDocument
                {
                    { "CourseId", new BsonDocument { { "$toObjectId", "$CourseId" } } },
                    { "UserId", new BsonDocument { { "$toObjectId", "$UserId" } } } // Ensure matching types
                }
            }
        },
        // Lookup stage to join with Course collection
        new BsonDocument
        {
            { "$lookup", new BsonDocument
                {
                    { "from", "Course" },
                    { "localField", "CourseId" },
                    { "foreignField", "_id" },
                    { "as", "courseDetails" }
                }
            }
        },
        // Unwind to flatten the joined data
        new BsonDocument
        {
            { "$unwind", "$courseDetails" }
        },

        new BsonDocument
        {
            { "$lookup", new BsonDocument
                {
                    { "from", "User" },
                    { "localField", "UserId" },
                    { "foreignField", "_id" },
                    { "as", "userDetails" }
                }
            }
        },
        new BsonDocument
        {
            { "$unwind", "$userDetails" }
        },
        // Project to include specific fields
        new BsonDocument
        {
            { "$project", new BsonDocument
                {
                    { "_id", 1 },
                    { "UserId", 1 },
                    { "CourseId", 1 },
                    { "Status", 1 },
                    { "UserMessage", 1 },
                    { "AppliedOn", 1 },
                    { "FollowUpDate", 1 },
                    { "FollowUpBy", 1 },
                    { "FollowUpHistory", 1 },
                    { "CourseDetails", new BsonDocument
                        {
                            { "_id", "$courseDetails._id" },
                            { "Name", "$courseDetails.Name" },
                            { "Category", "$courseDetails.Category" },
                            { "Institution", "$courseDetails.Institution" },
                            { "Location", "$courseDetails.Location" },
                            { "Duration", "$courseDetails.Duration" },
                            { "Summary", "$courseDetails.Summary" },
                            { "Trainer", "$courseDetails.Trainer" },
                            { "StartDate", "$courseDetails.StartDate" },
                        }
                    },
                    { "UserDetails", new BsonDocument
                        {
                            { "_id", "$userDetails._id" },
                            { "FirstName", "$userDetails.FirstName" },
                            { "LastName", "$userDetails.LastName" },
                            { "Email", "$userDetails.Email" },
                            { "PhoneNumber", "$userDetails.PhoneNumber" },
                            { "Location", "$userDetails.Location" },
                            { "DOB", "$userDetails.DOB" },
                            { "Gender", "$userDetails.Gender" },
                            { "UserType", "$userDetails.UserType" }
                        }
                    }
                }
            }
        }
    };

        // Execute the pipeline
        var result = await courseApplicationCollection.Aggregate<BsonDocument>(pipeline).ToListAsync();

        if (result == null || !result.Any())
        {
            return new List<CourseApplicationWithDetails>();
        }

        // Deserialize the result to the target model
        var courseApplicationsList = result.Select(x =>
            BsonSerializer.Deserialize<CourseApplicationWithDetails>(x)).ToList();

        return courseApplicationsList;
    }

    public async Task<UpdateResult> UpdateFollowUpHistory(string applicationId, CourseFollowUp newHistoryItem)
    {


        // Convert applicationId to ObjectId
        var objectId = ObjectId.Parse(applicationId);
        var courseApplicationCollection = _collection.Database.GetCollection<CourseApplication>("CourseApplication");
        // Define the update
        var update = Builders<CourseApplication>.Update
  .Push(x => x.FollowUpHistory, newHistoryItem)
  .Set(x => x.Status, newHistoryItem.ApplicationStatus);

        // Execute the update
        var result = await courseApplicationCollection.UpdateOneAsync(
            filter: Builders<CourseApplication>.Filter.Eq(x => x.Id, objectId),
            update: update
        );
        return result;
    }
    public async Task<UpdateResult> UpdateJobFollowUpHistory(string applicationId, FollowUp newHistoryItem)
    {


        // Convert applicationId to ObjectId
        var objectId = ObjectId.Parse(applicationId);
        var jobApplicationCollection = _collection.Database.GetCollection<JobApplication>("JobApplication");
        // Define the update
        var update = Builders<JobApplication>.Update
   .Push(x => x.FollowUpHistory, newHistoryItem)
   .Set(x => x.Status, newHistoryItem.Status);

        // Execute the update
        var result = await jobApplicationCollection.UpdateOneAsync(
            filter: Builders<JobApplication>.Filter.Eq(x => x.Id, objectId),
            update: update
        );
        return result;
    }

    public async Task<AdminDashCount> GetAdminDashCountAsync()
    {
        try
        {
            var jobCollection = _collection.Database.GetCollection<Jobs>("Jobs");
            var userCollection = _collection.Database.GetCollection<User>("User");
            var courseCollection = _collection.Database.GetCollection<Course>("Course");

            // Count documents in each collection
            var jobCountTask = jobCollection.CountDocumentsAsync(FilterDefinition<Jobs>.Empty);
            var userCountTask = userCollection.CountDocumentsAsync(FilterDefinition<User>.Empty);
            var courseCountTask = courseCollection.CountDocumentsAsync(FilterDefinition<Course>.Empty);

            // Get the latest entry from each collection based on _id
            var latestJobTask = jobCollection.Find(FilterDefinition<Jobs>.Empty)
                                              .Sort(Builders<Jobs>.Sort.Descending("_id"))
                                              .Limit(1)
                                              .FirstOrDefaultAsync();
            var latestUserTask = userCollection.Find(FilterDefinition<User>.Empty)
                                               .Sort(Builders<User>.Sort.Descending("_id"))
                                               .Limit(1)
                                               .FirstOrDefaultAsync();
            var latestCourseTask = courseCollection.Find(FilterDefinition<Course>.Empty)
                                                    .Sort(Builders<Course>.Sort.Descending("_id"))
                                                    .Limit(1)
                                                    .FirstOrDefaultAsync();

            // Wait for all tasks to complete
            await Task.WhenAll(jobCountTask, userCountTask, courseCountTask,
                               latestJobTask, latestUserTask, latestCourseTask);

            // Create and populate the dashboard count
            var adminDashCount = new AdminDashCount
            {
                DashItems = new List<DashItems>
            {
                new DashItems
                {
                    Title = "Job Listings",
                    ActiveCount = jobCountTask.Result,
                    InActiveCount = 0 // Replace with actual count logic if needed
                },
                new DashItems
                {
                    Title = "User Listings",
                    ActiveCount = userCountTask.Result,
                    InActiveCount = 0 // Replace with actual count logic if needed
                },
                new DashItems
                {
                    Title = "Course Listings",
                    ActiveCount = courseCountTask.Result,
                    InActiveCount = 0 // Replace with actual count logic if needed
                }
            },
                LatestItems = new List<LatestItems>
            {
                new LatestItems
                {
                    Title = latestJobTask != null ? $"New job posted: {latestJobTask.Result.JobTitle}" : "No jobs available"
                },
                new LatestItems
                {
                    Title = latestUserTask != null ? $"New user registered: {latestUserTask.Result.FirstName} {latestUserTask.Result.LastName}" : "No new users"
                },
                new LatestItems
                {
                    Title = latestCourseTask != null ? $"New course listed: {latestCourseTask.Result.Name}" : "No new courses"
                }
            }
            };

            return adminDashCount;
        }
        catch (Exception ex)
        {
            // Log the exception (assuming a logger is available)
            Console.WriteLine($"An error occurred while fetching dashboard data: {ex.Message}");
            return new AdminDashCount
            {
                DashItems = new List<DashItems>(),
                LatestItems = new List<LatestItems>()
            };
        }
    }


}

}
