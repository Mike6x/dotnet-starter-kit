using FSH.Framework.Core.Exceptions;

namespace FSH.Starter.WebApi.Elearning.Exceptions;
internal sealed class QuizNotFoundException : NotFoundException
{
    public QuizNotFoundException(Guid id)
        : base($"Quiz with id {id} not found")
    {
    }
}
