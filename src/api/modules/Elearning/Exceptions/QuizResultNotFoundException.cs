using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Elearning.Exceptions;
internal sealed class QuizResultNotFoundException : NotFoundException
{
    public QuizResultNotFoundException(Guid id)
        : base($"QuizResult with id {id} not found")
    {
    }
}
