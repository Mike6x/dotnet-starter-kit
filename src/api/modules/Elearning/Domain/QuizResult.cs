using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;

namespace FSH.Starter.WebApi.Elearning.Domain;
public class QuizResult : AuditableEntity, IAggregateRoot
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    // Student Id, Student Point, Used time, Time spent on taking the quiz
    public string? SId { get; set; }
    public decimal Sp { get; set; }
    public decimal Ut { get; set; }
    public string Fut { get; set; }

    // Quiz Title, Gained Score, Passing score, Passing score in percent, Total score, Time limit
    public string Qt { get; set; }
    public decimal Tp { get; set; }
    public decimal Ps { get; set; }
    public decimal Psp { get; set; }
    public decimal Tl { get; set; }

    // Quiz version, type : Graded
    public string V { get; set; }
    public string T { get; set; }

    public bool IsPass { get; private set; }
    public decimal? Rating { get; set; }
    public Guid QuizId { get; set; }
    public virtual Quiz Quiz { get; set; } = default!;

    public QuizResult(
        Guid quizId,
        DateTime startTime,
        DateTime endTime,
        string? userId,
        decimal earnedPoints,
        decimal userTimeSpend,
        string timeTakingQuiz,
        string quizTitle,
        decimal gainedScore,
        decimal passingScore,
        decimal passingScoreInPercent,
        decimal timeLimit,
        string quizVersion,
        string quizType,
        decimal? rating,
        bool isPass)
    {
        QuizId = quizId;

        StartTime = startTime;
        EndTime = endTime;

        SId = userId;
        Sp = earnedPoints;
        Ut = userTimeSpend;
        Fut = timeTakingQuiz;

        Qt = quizTitle;
        Tp = gainedScore;
        Ps = passingScore;
        Psp = passingScoreInPercent;
        Tl = timeLimit;

        V = quizVersion;
        T = quizType;
        Rating = rating;
        IsPass = isPass;
    }

    public QuizResult()
        : this(Guid.Empty, DateTime.UtcNow, DateTime.UtcNow, string.Empty, 0, 0, string.Empty,  string.Empty, 0, 0, 0, 0, string.Empty, string.Empty, null, false)
    {
    }

    public QuizResult Update(
        Guid? quizId,
        DateTime? startTime,
        DateTime? endTime,
        string? userId,
        decimal? earnedPoints,
        decimal? userTimeSpend,
        string? timeTakingQuiz,
        string? quizTitle,
        decimal? gainedScore,
        decimal? passingScore,
        decimal? passingScoreInPercent,
        decimal? timeLimit,
        string? quizVersion,
        string? quizType,
        decimal? rating)
    {
        if (quizId.HasValue && quizId.Value != Guid.Empty && !QuizId.Equals(quizId.Value)) QuizId = quizId.Value;
        if (startTime.HasValue && startTime.Value != DateTime.MinValue && !StartTime.Equals(startTime.Value)) StartTime = startTime.Value;
        if (endTime.HasValue && endTime.Value != DateTime.MinValue && !EndTime.Equals(endTime.Value)) EndTime = endTime.Value;

        if (userId is not null && SId?.Equals(userId) is not true) SId = userId;
        if (earnedPoints.HasValue && Sp != earnedPoints) Sp = earnedPoints.Value;
        if (userTimeSpend.HasValue && Ut != userTimeSpend) Ut = userTimeSpend.Value;
        if (timeTakingQuiz is not null && Fut?.Equals(timeTakingQuiz) is not true) Fut = timeTakingQuiz;

        if (quizTitle is not null && Qt?.Equals(quizTitle) is not true) Qt = quizTitle;
        if (gainedScore.HasValue && Tp != gainedScore) Tp = gainedScore.Value;
        if (passingScore.HasValue && Ps != passingScore) Ps = passingScore.Value;
        if (passingScoreInPercent.HasValue && Psp != passingScore) Psp = passingScoreInPercent.Value;
        if (timeLimit.HasValue && Tl != timeLimit) Tl = timeLimit.Value;

        if (quizVersion is not null && V?.Equals(quizVersion) is not true) V = quizVersion;
        if (quizType is not null && T?.Equals(quizType) is not true) T = quizType;

        if (rating.HasValue && rating != Rating) Rating = rating;

        IsPass = Sp >= Ps;
        return this;
    }
}
