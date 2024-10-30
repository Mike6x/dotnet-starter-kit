using Ardalis.Specification;
using FSH.Starter.WebApi.Elearning.Domain;

namespace FSH.Starter.WebApi.Elearning.Features.v1.QuizResults
{
    public sealed class QuizResultsByQuizIdSpec: Specification<QuizResult>
    {
        public QuizResultsByQuizIdSpec(Guid quizId) =>
            Query.Where(e => e.QuizId == quizId);
    }
}
