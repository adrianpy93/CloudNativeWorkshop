#region

using Dometrain.Monolith.Api.Students.Models;
using Dometrain.Monolith.Api.Students.Requests;

#endregion

namespace Dometrain.Monolith.Api.Students.Mappers;

public static class StudentMapper
{
    public static Student MapToStudent(this StudentRegistrationRequest request)
    {
        return new Student
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            FullName = request.FullName
        };
    }
}