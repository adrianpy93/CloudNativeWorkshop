#region

using Dometrain.Monolith.Api.Students.Interfaces;
using Dometrain.Monolith.Api.Students.Models;
using FluentValidation;

#endregion

namespace Dometrain.Monolith.Api.Students.Validators;

public class StudentValidator : AbstractValidator<Student>
{
    private readonly IStudentRepository _studentRepository;

    public StudentValidator(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.Email).NotEmpty().EmailAddress()
            .MustAsync(ValidateEmail)
            .WithMessage("An account with this email already exists");
    }

    private async Task<bool> ValidateEmail(Student student, string email, CancellationToken token = default)
    {
        var existingStudent = await _studentRepository.GetByEmailAsync(email);

        if (existingStudent is not null) return existingStudent.Id == student.Id;

        return existingStudent is null;
    }
}