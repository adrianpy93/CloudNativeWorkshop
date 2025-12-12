#region

using System.Text.Json;
using StackExchange.Redis;

#endregion

namespace Dometrain.Monolith.Api.Courses;

public class CachedCourseRepository : ICourseRepository
{
    private readonly ICourseRepository _courseRepository;
    private readonly IConnectionMultiplexer _multiplexer;

    public CachedCourseRepository(ICourseRepository courseRepository, IConnectionMultiplexer multiplexer)
    {
        _courseRepository = courseRepository;
        _multiplexer = multiplexer;
    }

    public async Task<Course?> CreateAsync(Course course)
    {
        var created = await _courseRepository.CreateAsync(course);
        if (created is null) return null;

        var db = _multiplexer.GetDatabase();
        var serializedCourse = JsonSerializer.Serialize(course);
        var batch = new KeyValuePair<RedisKey, RedisValue>[]
        {
            new($"course:id:{course.Id.ToString()}", serializedCourse),
            new($"course:slug:{course.Slug}", course.Id.ToString())
        };

        await db.StringSetAsync(batch);
        return created;
    }

    public async Task<Course?> GetByIdAsync(Guid id)
    {
        var db = _multiplexer.GetDatabase();
        var cachedCourse = await db.StringGetAsync($"course:id:{id}");
        if (!cachedCourse.IsNull) return JsonSerializer.Deserialize<Course>(cachedCourse.ToString());

        var course = await _courseRepository.GetByIdAsync(id);
        if (course is null) return course;

        var serializedCourse = JsonSerializer.Serialize(course);

        var batch = new KeyValuePair<RedisKey, RedisValue>[]
        {
            new($"course:id:{course.Id.ToString()}", serializedCourse),
            new($"course:slug:{course.Slug}", course.Id.ToString())
        };
        await db.StringSetAsync(batch);
        return course;
    }

    public async Task<Course?> GetBySlugAsync(string slug)
    {
        var db = _multiplexer.GetDatabase();
        var cachedCourseKey = await db.StringGetAsync($"course:slug:{slug}");

        if (!cachedCourseKey.IsNull) return await GetByIdAsync(Guid.Parse(cachedCourseKey.ToString()));

        var course = await _courseRepository.GetBySlugAsync(slug);
        if (course is null) return course;

        var serializedCourse = JsonSerializer.Serialize(course);
        var batch = new KeyValuePair<RedisKey, RedisValue>[]
        {
            new($"course:id:{course.Id}", serializedCourse),
            new($"course:slug:{course.Slug}", course.Id.ToString())
        };
        await db.StringSetAsync(batch);
        return course;
    }

    public Task<IEnumerable<Course>> GetAllAsync(string nameFilter, int pageNumber, int pageSize)
    {
        return _courseRepository.GetAllAsync(nameFilter, pageNumber, pageSize);
    }

    public async Task<Course?> UpdateAsync(Course course)
    {
        var updated = await _courseRepository.UpdateAsync(course);
        if (updated is null) return updated;

        var db = _multiplexer.GetDatabase();
        var serializedCourse = JsonSerializer.Serialize(course);
        var batch = new KeyValuePair<RedisKey, RedisValue>[]
        {
            new($"course:id:{course.Id.ToString()}", serializedCourse),
            new($"course:slug:{course.Slug}", course.Id.ToString())
        };
        await db.StringSetAsync(batch);
        return updated;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleted = await _courseRepository.DeleteAsync(id);

        if (!deleted) return deleted;

        var db = _multiplexer.GetDatabase();
        var cachedCourseString = await db.StringGetAsync($"course:id:{id}");
        if (cachedCourseString.IsNull) return deleted;

        var course = JsonSerializer.Deserialize<Course>(cachedCourseString.ToString())!;
        var deletedCache = await db.KeyDeleteAsync([$"course:id:{id}", $"course:slug:{course.Slug}"]);
        return deletedCache > 0;
    }
}