#region

using Dapper;
using Npgsql;

#endregion

namespace Dometrain.Monolith.Api.Database;

public class DbInitializer(NpgsqlDataSource dataSource)
{
    public async Task InitializeAsync()
    {
        // Retry logic to wait for PostgreSQL to be ready
        const int maxRetries = 10;
        const int delayMs = 1000;

        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                await using var connection = await dataSource.OpenConnectionAsync();

        const string script = """
                              create table if not exists students
                              (
                                  id            UUID primary key,
                                  email         text not null,
                                  fullname      text not null,
                                  password_hash text not null
                              );

                              create unique index if not exists students_email_index
                                  on students
                                  using btree(email);
                                  
                              -- create api user ApiUserId

                              insert into students (id, email, fullname, password_hash)
                              values ('005d25b1-bfc8-4391-b349-6cec00d1416c', 'admin@dometrain.com', 'Api Admin', 'AQAAAAIAAYagAAAAEEBJk5xkvY3cVoWtMkbUowYCRzvpRQ76ecv7U1aiw/9RYlarhzagQvCLeK/1lb8v9g==')
                              on conflict do nothing;

                              create table if not exists courses
                              (
                                  id             UUID primary key,
                                  "name"         text not null,
                                  "description"  text not null,
                                  slug           text not null,
                                  author         text not null
                              );

                              create unique index if not exists courses_slug_index
                                  on courses
                                  using btree(slug);

                              create table if not exists shopping_cart_items
                              (
                                  student_id     UUID not null references students (id),
                                  course_id      UUID not null references courses(id),
                                  primary key (student_id, course_id)
                              );

                              create table if not exists enrollments
                              (
                                  student_id     UUID not null references students (id),
                                  course_id      UUID not null references courses(id),
                                  primary key (student_id, course_id)
                              );

                              create table if not exists orders
                              (
                                  id              UUID primary key,
                                  student_id      UUID not null references students (id),
                                  created_at_utc  timestamp with time zone
                              );

                              create table if not exists order_items
                              (
                                  order_id       UUID not null references orders(id),
                                  course_id      UUID not null references courses(id),
                                  primary key (order_id, course_id)
                              );

                              """;

                await connection.ExecuteAsync(script);
                return; // Success, exit retry loop
            }
            catch (Exception) when (i < maxRetries - 1)
            {
                // PostgreSQL not ready yet, wait and retry
                Console.WriteLine($"Database not ready, retrying in {delayMs}ms... ({i + 1}/{maxRetries})");
                await Task.Delay(delayMs);
            }
        }
    }
}