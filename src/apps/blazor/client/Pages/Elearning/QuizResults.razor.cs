using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Shared;
using Mapster;
using Microsoft.AspNetCore.Components;

namespace FSH.Starter.Blazor.Client.Pages.Elearning;

public partial class QuizResults : ComponentBase
{
    [Inject]
    protected IApiClient ApiClient { get; set; } = default!;

    private EntityTable<QuizResultDto, Guid, QuizResultViewModel> _table = default!;
    protected EntityServerTableContext<QuizResultDto, Guid, QuizResultViewModel> Context { get; set; } = default!;

    protected override void OnInitialized()
    {
        Context = new(
            entityName: "QuizResult",
            entityNamePlural: "QuizResults",
            entityResource: FshResources.QuizResults,
            fields: new()
            {
                // new(QuizResult => QuizResult.Id, "Id", "Id"),
                new(QuizResult => QuizResult.QuizCode, "Quiz Code", "Quiz.Code"),
                // new(QuizResult => QuizResult.QuizName, "Quiz Name", "Quiz.Name"),

                new(QuizResult => QuizResult.Qt, "Exam Title", "Qt"),

                new(QuizResult => QuizResult.SId, "Student Id", "SId"),
                new(QuizResult => QuizResult.Sp, "Student Point", "Sp"),

                new(QuizResult => QuizResult.Psp, "Require Score in %", "Psp"),
                // new(QuizResult => QuizResult.Ps, "Passing Score", "Ps"),
                new(QuizResult => QuizResult.Tp, "Total Score", "Tp"),


                new(QuizResult => QuizResult.StartTime.ToLocalTime().ToString("yyyy/MM/dd HH:mmm"), "StartTime", "StartTime"),
                new(QuizResult => QuizResult.EndTime.ToLocalTime().ToString("yyyy/MM/dd HH:mmm"), "EndTime", "EndTime"),

                // new(QuizResult => QuizResult.Ut, "Used Time in Seconds", "Ut"),
                // new(QuizResult => QuizResult.Tl, "Time Limit", "Tl"),

                new(QuizResult => QuizResult.Rating, "Rating", "Rating"),
                new(QuizResult => QuizResult.IsPass, "Is Passed", Type: typeof(bool)),

            },
            enableAdvancedSearch: true,
            idFunc: item => item.Id,
            exportFunc: async filter =>
            {
                var dataFilter = filter.Adapt<ExportQuizResultsRequest>();

                dataFilter.QuizId = SearchQuizId == Guid.Empty ? null : SearchQuizId;
                dataFilter.UserId = SearchUserString;
                dataFilter.IsPass = SearchIsPass;

                return await ApiClient.ExportQuizResultsEndpointAsync("1", dataFilter);
            },
            importFunc: async (fileUploadModel, isUpdate) =>
                await ApiClient.ImportQuizResultsEndpointAsync("1", isUpdate, fileUploadModel),
            searchFunc: async filter =>
            {
                var dataFilter = filter.Adapt<SearchQuizResultsRequest>();

                dataFilter.QuizId = SearchQuizId == Guid.Empty ? null : SearchQuizId;
                dataFilter.IsPass = SearchIsPass;

                var result = await ApiClient.SearchQuizResultsEndpointAsync("1", dataFilter);

                return result.Adapt<PaginationResponse<QuizResultDto>>();
            },
            createFunc: async itemModel =>
            {
                await ApiClient.CreateQuizResultEndpointAsync("1", itemModel.Adapt<CreateQuizResultCommand>());
            },
            updateFunc: async (id, itemModel) =>
            {
                 await ApiClient.UpdateQuizResultEndpointAsync("1", id, itemModel.Adapt<UpdateQuizResultCommand>());
            },
            deleteFunc: async id => await ApiClient.DeleteQuizResultEndpointAsync("1", id));
    }


    #region Advanced Search

   private Guid _searchQuizId;
    private Guid SearchQuizId
    {
        get => _searchQuizId;
        set
        {
            _searchQuizId = value;
            _ = _table?.ReloadDataAsync();
        }
    }


    private bool? _searchIsPass;
    private bool? SearchIsPass
    {
        get => _searchIsPass;
        set
        {
            _searchIsPass = value;
            _ = _table?.ReloadDataAsync();
        }
    }


    private string? SearchUserString { get; set; }
    private void OnUserStringChanged()
    {
        _ = _table?.ReloadDataAsync();
    }
    #endregion

    /// <summary>
    /// View Student info.
    /// </summary>
    /// <param name="userId"></param>
    private void ViewProfile(string userId)
    {
        // if (!string.IsNullOrEmpty(userId))
        Navigation.NavigateTo($"/users/{userId}/profile");
    }
}

public class QuizResultViewModel : UpdateQuizResultCommand
{
}
