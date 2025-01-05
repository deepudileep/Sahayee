using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;
using Sahayee.Helper;
using Sahayee.Models.DB;

public class MongoDBChangeWatcher : BackgroundService
{
    private readonly IMongoCollection<Jobs> _jobsCollection;
    private readonly IMongoCollection<Course> _courseCollection;
    private readonly IMongoCollection<CourseApplication> _courseApplicationCollection;
    private readonly IMongoCollection<JobApplication> _jobApplicationCollection;
    private readonly IHubContext<NotificationHub> _hubContext;

    // Separate last poll times for each collection
    private DateTime _lastPollTimeJobs = DateTime.Now;
    private DateTime _lastPollTimeCourse = DateTime.Now;
    private DateTime _lastPollTimeCourseApplication = DateTime.Now;
    private DateTime _lastPollTimeJobApplication = DateTime.Now;

    public MongoDBChangeWatcher(IMongoClient mongoClient, IHubContext<NotificationHub> hubContext, IConfiguration configuration)
    {
        var databaseName = configuration.GetValue<string>("MongoDb:DatabaseName");
        var database = mongoClient.GetDatabase(databaseName);

        // Initialize the collections
        _jobsCollection = database.GetCollection<Jobs>("Jobs");
        _courseCollection = database.GetCollection<Course>("Course");
        _courseApplicationCollection = database.GetCollection<CourseApplication>("CourseApplication");
        _jobApplicationCollection = database.GetCollection<JobApplication>("JobApplication");

        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Poll for changes in each collection using appropriate filters
            var jobsChanges = await _jobsCollection.Find(Builders<Jobs>.Filter.Gt(j => j.LastModified, _lastPollTimeJobs))
                .ToListAsync(stoppingToken);
            var courseChanges = await _courseCollection.Find(Builders<Course>.Filter.Gt(c => c.LastModified, _lastPollTimeCourse))
                .ToListAsync(stoppingToken);
            var courseAppChanges = await _courseApplicationCollection.Find(Builders<CourseApplication>.Filter.Gt(ca => ca.LastModified, _lastPollTimeCourseApplication))
                .ToListAsync(stoppingToken);
            var jobAppChanges = await _jobApplicationCollection.Find(Builders<JobApplication>.Filter.Gt(ja => ja.LastModified, _lastPollTimeJobApplication))
                .ToListAsync(stoppingToken);

            // Notify clients about changes for each collection
            if (jobsChanges.Any())
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", GetCollectionMessage("Jobs", jobsChanges.Count));
                _lastPollTimeJobs = DateTime.Now;  // Update last poll time for Jobs
            }

            if (courseChanges.Any())
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", GetCollectionMessage("Course", courseChanges.Count));
                _lastPollTimeCourse = DateTime.Now;  // Update last poll time for Course
            }

            if (courseAppChanges.Any())
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", GetCollectionMessage("Course Applications", courseAppChanges.Count));
                _lastPollTimeCourseApplication = DateTime.Now;  // Update last poll time for Course Applications
            }

            if (jobAppChanges.Any())
            {
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", GetCollectionMessage("Job Applications", jobAppChanges.Count));
                _lastPollTimeJobApplication = DateTime.Now;  // Update last poll time for Job Applications
            }

            // Wait for 10 seconds before polling again
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    // Helper method to generate the collection message with FontAwesome icon
    private string GetCollectionMessage(string collectionName, int changeCount)
    {
        string icon = collectionName switch
        {
            "Jobs" => "<a href='/Job/Jobs'><i class='fas fa-briefcase'></i>",
            "Course" => "<a href='/Course/Courses'><i class='fas fa-chalkboard-teacher'></i>",
            "Course Applications" => "<a href=''><i class='fas fa-book-reader'></i>",
            "Job Applications" => "<a href=''><i class='fas fa-file-alt'></i>",
            _ => "<a href=''><i class='fas fa-info-circle'></i>"
        };

        return $"{icon} New updates {changeCount} in the {collectionName}</a>";
    }
}
