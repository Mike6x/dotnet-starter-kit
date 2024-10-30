using Ardalis.Specification;
using FSH.Starter.WebApi.Elearning.Domain;

namespace FSH.Starter.WebApi.Elearning.Features.v1.Quizs
{
    public sealed class QuizByIdSpec : Specification<Quiz, QuizDto>, ISingleResultSpecification<Quiz>
    {
        public QuizByIdSpec(Guid id) =>
            Query
                .Where(e => e.Id == id);
    }

    public sealed class QuizByCodeSpec : Specification<Quiz>, ISingleResultSpecification<Quiz>
    {
        public QuizByCodeSpec(string code) =>
            Query
                .Where(e => e.Code == code);
    }

    public sealed class QuizByNameSpec : Specification<Quiz>, ISingleResultSpecification<Quiz>
    {
        public QuizByNameSpec(string name) =>
            Query
                .Where(e => e.Name == name);
    }

}
