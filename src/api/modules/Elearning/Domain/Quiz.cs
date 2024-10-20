using FSH.Framework.Core.Domain;
using FSH.Framework.Core.Domain.Contracts;
using FSH.Starter.WebApi.Elearning.Domain.Events;
using FSH.Starter.WebApi.Setting.Domain;

namespace FSH.Starter.WebApi.Elearning.Domain;
public class Quiz :  AuditableEntity, IAggregateRoot
{
    public int Order { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    
    public Uri? QuizUrl { get; private set; } 
    public DateTime FromDate { get; private set; }
    public DateTime ToDate { get; private set; }
    
    /// <summary>
    /// Type = Quiz, content only have video
    /// Type = ELeaning, Content have MP4 video and quiz
    /// Type = Servey
    /// </summary>
    public Guid QuizTypeId { get; private set; }
    // public virtual Dimension QuizType { get; private set; } = default
    /// <summary>
    /// Topic like Orientation, game ...
    /// </summary>
    public Guid QuizTopicId { get; private set; }
    // public virtual Dimension QuizTopic { get; private set; } = default!
    /// <summary>
    /// Quiz Mode = Practice, do any time
    /// Quiz Mode = Pretest, do until get pass
    /// Quiz Mode = Test, do one time
    /// </summary>
    public Guid QuizModeId { get; private set; }
    // public virtual Dimension QuizMode { get; private set; } = default!

    public decimal? Price { get; private set; }
    public int? Sale { get; private set; }
    public int? RatingCount { get; private set; }
    public decimal? Rating { get; private set; }

    public Quiz(
        int order,
        string code,
        string name,
        string? description,
        bool isActive,
        
        Uri? quizUrl,
        DateTime? fromDate,
        DateTime? toDate,
        
        Guid quizTypeId,
        Guid quizTopicId,
        Guid quizModeId,
        
        decimal? price,
        int? sale,
        int? ratingCount,
        decimal? rating)
    {
        Order = order;
        Code = code;
        Name = name;
        Description = description;
        IsActive = toDate >= DateTime.Today && isActive;
        
        QuizUrl = quizUrl;
        FromDate = fromDate ?? DateTime.MinValue;
        ToDate = toDate ?? DateTime.UtcNow;
       
        QuizTypeId = quizTypeId;
        QuizTopicId = quizTopicId;
        QuizModeId = quizModeId;
        
        Sale = sale;
        Price = price;
        RatingCount = ratingCount;
        Rating = rating;
    }

    public Quiz()
    : this(
        0, 
        string.Empty, 
        string.Empty, 
        string.Empty, 
        false,
        null, 
        DateTime.MinValue, 
        DateTime.UtcNow, 
        Guid.Empty, 
        Guid.Empty, 
        Guid.Empty, 
        null, 
        null, 
        null, 
        null)
    {
    }
    
    public static Quiz Create(
        int? order,
        string code,
        string name,
        string? description,
        bool? isActive,
        Uri? quizUrl,
        DateTime? fromDate,
        DateTime? toDate,
        Guid quizTypeId,
        Guid quizTopicId,
        Guid quizModeId,
        decimal? price,
        int? sale,
        int? ratingCount,
        decimal? rating)
    {
        var item = new Quiz
        {
            Order = order ?? 0,
            Code = code,
            Name = name,
            Description = description,
            IsActive = isActive ?? true,
            
            QuizUrl = quizUrl,
            FromDate = fromDate ?? DateTime.MinValue,
            ToDate = toDate ?? DateTime.UtcNow,
            
            QuizTypeId = quizTypeId,
            QuizTopicId = quizTopicId,
            QuizModeId = quizModeId,
           
            Price = price,
            Sale = sale,
            RatingCount = ratingCount,
            Rating = rating,
        };
        
   item.QueueDomainEvent(new QuizCreated(item.Id, item.Order, item.Code, item.Name, item.Description, item.IsActive,
            item.QuizUrl, item.FromDate, item.ToDate, 
            item.QuizTypeId, item.QuizTopicId, item.QuizModeId, 
            item.Price, item.Sale, item.RatingCount,item.Rating));

        QuizMetrics.Created.Add(1);

        return item;
    }

    public Quiz Update(
    int? order,
    string? code,
    string? name,
    string? description,
    bool? isActive,
    Uri? quizUrl,
    DateTime? fromDate,
    DateTime? toDate,
    Guid? quizTypeId,
    Guid? quizTopicId,
    Guid? quizModeId,
    decimal? price,
    int? sale,
    int? ratingCount,
    decimal? rating)
    {
        if (order.HasValue && Order != order) Order = order.Value;
        if (code is not null && !Code.Equals(code, StringComparison.Ordinal)) Code = code;
        if (name is not null && !Name.Equals(name, StringComparison.Ordinal)) Name = name;
        if (description is not null && Description?.Equals(description,StringComparison.Ordinal) is not true) Description = description;
        if (isActive is not null && !IsActive.Equals(isActive)) IsActive = (bool)isActive;
        
        // if (quizUrl is not null && !quizUrl.Equals(QuizUrl))
        QuizUrl = quizUrl;
        
        if (fromDate.HasValue && fromDate.Value != DateTime.MinValue && !FromDate.Equals(fromDate.Value)) FromDate = fromDate.Value;
        if (toDate.HasValue && toDate.Value != DateTime.MinValue && !ToDate.Equals(toDate.Value)) ToDate = toDate.Value;

        IsActive = ToDate >= DateTime.Today && IsActive;

        if (quizTypeId.HasValue && quizTypeId.Value != Guid.Empty && !QuizTypeId.Equals(quizTypeId.Value)) QuizTypeId = quizTypeId.Value;
        if (quizTopicId.HasValue && quizTopicId.Value != Guid.Empty && !QuizTopicId.Equals(quizTopicId.Value)) QuizTopicId = quizTopicId.Value;
        if (quizModeId.HasValue && quizModeId.Value != Guid.Empty && !QuizModeId.Equals(quizModeId.Value)) QuizModeId = quizModeId.Value;

        if (sale.HasValue && sale != Sale) Sale = sale;
        if (price.HasValue && price != Price) Price = price;
        if (ratingCount.HasValue && ratingCount != RatingCount) RatingCount = ratingCount;
        if (rating.HasValue && rating != Rating) Rating = rating;
        
       QueueDomainEvent(new QuizUpdated(this));

        return this;
    }
    
    //
    // public Quiz ClearQuizUrl()
    // {
    //     QuizUrl = null;
    //     return this;
    // }
    
}
