using System.Globalization;
using FSH.Starter.Blazor.Client.Components.EntityTable;
using FSH.Starter.Blazor.Infrastructure.Api;
using FSH.Starter.Blazor.Shared;
using Mapster;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.VisualBasic;
using MudBlazor;

namespace FSH.Starter.Blazor.Client.Pages.Elearning;

public partial class Quizs : ComponentBase
{
    [CascadingParameter]
    protected Task<AuthenticationState> AuthState { get; set; } = default!;
    [Inject]
    protected IApiClient ApiClient { get; set; } = default!;

    protected EntityServerTableContext<QuizDto, Guid, QuizViewModel> Context { get; set; } = default!;

    private EntityTable<QuizDto, Guid, QuizViewModel> _table = default!;

    protected override async void OnInitialized()
    {
        if ((await AuthState).User is { } user)
        {
            _currentUserId = user.GetUserId() ?? string.Empty;
        }

        Context = new(
            entityName: "Quiz",
            entityNamePlural: "Quizs",
            entityResource: FshResources.Quizs,
            fields: new()
            {
                new(item => item.Order,"Order", "Order"),
                new(item => item.Code, "Code", "Code"),
                new(item => item.Name, "Name", "Name"),
                // new(item => item.Description, "Description", "Description"),

                new(item => item.FromDate.ToLocalTime().ToString("yyyy/MM/dd HH:mmm"), "FromDate", "FromDate", Type: typeof(DateTime)),
                new(item => item.ToDate.ToLocalTime().ToString("yyyy/MM/dd HH:mmm"), "ToDate", "ToDate", Type: typeof(DateTime)),

                // new(item => item.QuizTopicName, "Topic", "itemTopic.Name"),
                // new(item => item.QuizModeName, "Mode", "itemMode.Name"),
                // new(item => item.QuizTypeName, "Type", "itemType.name"),

                // new(item => item.itemPath, L["itemPath"], "itemPath"),
                new(item => item.Price, "Price", "Price"),
                new(item => item.Sale, "Sale", "Sale"),
                new(item => item.Rating, "Rating", "Rating"),
                new(item => item.RatingCount, "RatingCount", "RatingCount"),

                new(item => item.IsActive, "Active", Type: typeof(bool)),
            },
            enableAdvancedSearch: false,
            idFunc: item => item.Id,
            exportFunc: async filter =>
            {
                var dataFilter = filter.Adapt<ExportQuizsRequest>();
   
                dataFilter.QuizTypeId = SearchQuizTypeId == Guid.Empty ? null : SearchQuizTypeId;
                dataFilter.QuizTopicId = SearchQuizTopicId == Guid.Empty ? null : SearchQuizTopicId;
                dataFilter.QuizModeId = SearchQuizModeId == Guid.Empty ? null : SearchQuizModeId;

                dataFilter.FromDate = SearchFromDate;
                dataFilter.ToDate = SearchToDate;
                dataFilter.IsActive = SearchIsActive;

                return await ApiClient.ExportQuizsEndpointAsync("1", dataFilter);
            },
            importFunc: async (fileUploadModel, isUpdate) =>
                await ApiClient.ImportQuizsEndpointAsync("1", isUpdate, fileUploadModel),
            searchFunc: async filter =>
            {
                var dataFilter = filter.Adapt<SearchQuizsRequest>();
                
                dataFilter.QuizTypeId = SearchQuizTypeId == Guid.Empty ? null : SearchQuizTypeId;
                dataFilter.QuizTopicId = SearchQuizTopicId == Guid.Empty ? null : SearchQuizTopicId;
                dataFilter.QuizModeId = SearchQuizModeId == Guid.Empty ? null : SearchQuizModeId;

                dataFilter.FromDate = SearchFromDate;
                dataFilter.ToDate = SearchToDate;
                dataFilter.IsActive = SearchIsActive;

                var result = await ApiClient.SearchQuizsEndpointAsync("1", dataFilter);

                return result.Adapt<PaginationResponse<QuizDto>>();
            },
            createFunc: async itemModel =>
            {
                if (!string.IsNullOrEmpty(itemModel.QuizInBytes))
                {
                    itemModel.MediaUpload = new FileUploadCommand()
                    {
                        Data = itemModel.QuizInBytes,
                        Extension = itemModel.QuizExtension ?? string.Empty,
                        Name = $"{itemModel.Code}"

                        // Name = $"{itemModel.Name}_{Guid.NewGuid():N}"
                    };
                }

                itemModel.FromDate = DateTimeHelper.Local2Utc(itemModel.LocalFromDate ?? DateTime.Now, itemModel.LocalTimeOfFromDate ?? TimeSpan.Zero);
                itemModel.ToDate = DateTimeHelper.Local2Utc(itemModel.LocalToDate ?? DateTime.Now, itemModel.LocalTimeOfToDate ?? TimeSpan.Zero);
                
                await ApiClient.CreateQuizEndpointAsync("1", itemModel.Adapt<CreateQuizCommand>());
                itemModel.QuizInBytes = string.Empty;
            },
            updateFunc: async (id, itemModel) =>
            {
                if (!string.IsNullOrEmpty(itemModel.QuizInBytes))
                {
                    itemModel.DeleteCurrentQuiz = true;
                    itemModel.MediaUpload = new FileUploadCommand()
                    {
                        Data = itemModel.QuizInBytes,
                        Extension = itemModel.QuizExtension ?? string.Empty,
                        Name = $"{itemModel.Code}"

                        // Name = $"{itemModel.Name}_{Guid.NewGuid():N}"
                    };
                }

                itemModel.FromDate = DateTimeHelper.Local2Utc(itemModel.LocalFromDate ?? DateTime.Now, itemModel.LocalTimeOfFromDate ?? TimeSpan.Zero);
                itemModel.ToDate = DateTimeHelper.Local2Utc(itemModel.LocalToDate ?? DateTime.Now, itemModel.LocalTimeOfToDate ?? TimeSpan.Zero);
                
                await ApiClient.UpdateQuizEndpointAsync("1", id, itemModel.Adapt<UpdateQuizCommand>());
                itemModel.QuizInBytes = string.Empty;
            },
            deleteFunc: async id => await ApiClient.DeleteQuizEndpointAsync("1", id));

    }

    // Take a Quizs

    private string _currentUserId = string.Empty;
    // private string QuizUrl => Config[ConfigNames.ApiBaseUrl] + Context.AddEditModal.RequestModel.QuizUrl
    //                                                          + "/index.html?QuizId=" + Context.AddEditModal.RequestModel.Id
    //                                                          + "&SId=" + _currentUserId
    /// <summary>
    /// Input information QuizId and Student Id in the Quiz result flow rules from Ispring gihub.
    /// </summary>
    private string QuizUrl =>  Context.AddEditModal.RequestModel.QuizUrl
                                                             + "/index.html?QuizId=" + Context.AddEditModal.RequestModel.Id
                                                             + "&SId=" + _currentUserId;
    
    // TODO : Make this as a shared service or something? Since it's used by Profile Component also for now, and literally any other component that will have image upload.
    // The new service should ideally return $"data:{ApplicationConstants.StandardImageFormat};base64,{Convert.ToBase64String(buffer)}"


    #region Quizs Upload
    private IBrowserFile? UploadFile { get; set; }
    public void ClearQuizInBytes()
    {
        UploadFile = null;
        Context.AddEditModal.RequestModel.QuizInBytes = string.Empty;

        Context.AddEditModal.RequestModel.DeleteCurrentQuiz = false;
        Context.AddEditModal.ForceRender();
    }

    public void SetDeleteCurrentQuizFlag()
    {
        Context.AddEditModal.RequestModel.QuizInBytes = string.Empty;
        Context.AddEditModal.RequestModel.QuizPath = string.Empty;

        Context.AddEditModal.RequestModel.DeleteCurrentQuiz = true;
        Context.AddEditModal.ForceRender();
    }

    private async Task UploadFiles(InputFileChangeEventArgs e)
    {
        if (e.File != null)
        {
            UploadFile = e.File;
            string? extension = Path.GetExtension(e.File.Name);
            if (!AppConstants.SupportedQuizMediaFormats.Contains(extension.ToLower(CultureInfo.CurrentCulture)))
            {
                Toast.Add("QuizMedia Format Not Supported.", Severity.Error);
                UploadFile = null;
                return;
            }

            Context.AddEditModal.RequestModel.QuizExtension = extension;
            byte[]? buffer = new byte[UploadFile.Size];
            await UploadFile.OpenReadStream(AppConstants.MaxQuizMediaFileSize).ReadAsync(buffer);
            Context.AddEditModal.RequestModel.QuizInBytes = $"data:{AppConstants.StandardQuizMediaFormat};base64,{Convert.ToBase64String(buffer)}";

            Context.AddEditModal.RequestModel.DeleteCurrentQuiz = true;

            Context.AddEditModal.ForceRender();
        }
    }
    #endregion

    #region Advanced Search

    private Guid _searchQuizTypeId;
    private Guid SearchQuizTypeId
    {
        get => _searchQuizTypeId;
        set
        {
            _searchQuizTypeId = value;
            _ = _table?.ReloadDataAsync();
        }
    }

    private Guid _searchQuizTopicId;
    private Guid SearchQuizTopicId
    {
        get => _searchQuizTopicId;
        set
        {
            _searchQuizTopicId = value;
            _ = _table?.ReloadDataAsync();
        }
    }

    private Guid _searchQuizModeId;
    private Guid SearchQuizModeId
    {
        get => _searchQuizModeId;
        set
        {
            _searchQuizModeId = value;
            _ = _table?.ReloadDataAsync();
        }
    }

    private DateTime? _searchFromDate;
    private DateTime? SearchFromDate
    {
        get => _searchFromDate;
        set
        {
            _searchFromDate = value;
            _ = _table?.ReloadDataAsync();
        }
    }

    private DateTime? _searchToDate;
    private DateTime? SearchToDate
    {
        get => _searchToDate;
        set
        {
            _searchToDate = value;
            _ = _table?.ReloadDataAsync();
        }
    }


    private bool? _searchIsActive;
    private bool? SearchIsActive
    {
        get => _searchIsActive;
        set
        {
            _searchIsActive = value;
            _ = _table?.ReloadDataAsync();
        }
    }
    #endregion
}

public class QuizViewModel : UpdateQuizCommand
{
    #region File Upload
    public string? QuizPath { get; set; }
    public string? QuizInBytes { get; set; }
    public string? QuizExtension { get; set; }
    #endregion

    #region Date and Time Picker

    private TimeSpan? _localTimeOfFromDate;
    public TimeSpan? LocalTimeOfFromDate
    {
        get
        {
            if (_localTimeOfFromDate == null && FromDate != null)
                _localTimeOfFromDate = FromDate!.Value.ToLocalTime().TimeOfDay;

            return _localTimeOfFromDate;
        }
        set
        {
            _localTimeOfFromDate = value;
        }
    }

    private DateTime? _localFromDate;
    public DateTime? LocalFromDate
    {
        get
        {
            if (_localFromDate == null && FromDate != null)
                _localFromDate = FromDate!.Value.ToLocalTime();

            return _localFromDate;
        }
        set
        {
            _localFromDate = value;
        }
    }

    private TimeSpan? _localTimeOfToDate;
    public TimeSpan? LocalTimeOfToDate
    {
        get
        {
            if (_localTimeOfToDate == null && ToDate != null)
                _localTimeOfToDate = ToDate!.Value.ToLocalTime().TimeOfDay;

            return _localTimeOfToDate;
        }
        set
        {
            _localTimeOfToDate = value;
        }
    }

    private DateTime? _localToDate;
    public DateTime? LocalToDate
    {
        get
        {
            if (_localToDate == null && ToDate != null)
                _localToDate = ToDate.Value.ToLocalTime();

            return _localToDate;
        }
        set
        {
            _localToDate = value;
        }
    }
    #endregion
}
