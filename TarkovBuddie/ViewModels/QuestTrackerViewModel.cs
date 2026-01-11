using System.Collections.ObjectModel;
using System.Windows.Input;
using TarkovBuddie.Models;
using TarkovBuddie.Services;

namespace TarkovBuddie.ViewModels;

public class QuestTrackerViewModel : ViewModelBase
{
    private readonly QuestService _questService;
    private ObservableCollection<QuestViewModel> _quests = new();
    private ObservableCollection<QuestViewModel> _filteredQuests = new();
    private bool _hideCompleted = false;
    private bool _onlyKappa = false;
    private string _searchText = string.Empty;
    private QuestViewModel? _selectedQuest;
    private string _questsLabel = string.Empty;
    private string _kappaLabel = string.Empty;

    public ObservableCollection<QuestViewModel> FilteredQuests => _filteredQuests;

    public bool HideCompleted
    {
        get => _hideCompleted;
        set
        {
            if (SetProperty(ref _hideCompleted, value))
            {
                RefreshFilteredQuests();
            }
        }
    }

    public bool OnlyKappa
    {
        get => _onlyKappa;
        set
        {
            if (SetProperty(ref _onlyKappa, value))
            {
                RefreshFilteredQuests();
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (SetProperty(ref _searchText, value))
            {
                RefreshFilteredQuests();
            }
        }
    }

    public QuestViewModel? SelectedQuest
    {
        get => _selectedQuest;
        set => SetProperty(ref _selectedQuest, value);
    }

    public string QuestsLabel
    {
        get => _questsLabel;
        set => SetProperty(ref _questsLabel, value);
    }

    public string KappaLabel
    {
        get => _kappaLabel;
        set => SetProperty(ref _kappaLabel, value);
    }

    public ICommand ToggleCompletionCommand { get; }
    public ICommand TogglePinCommand { get; }

    public QuestTrackerViewModel()
    {
        _questService = new QuestService();

        ToggleCompletionCommand = new RelayCommand(ExecuteToggleCompletion);
        TogglePinCommand = new RelayCommand(ExecuteTogglePin);

        LoadQuests();
    }

    private void LoadQuests()
    {
        _quests.Clear();
        _filteredQuests.Clear();

        var questsData = _questService.LoadQuests();
        var tracker = _questService.LoadQuestProgress();

        foreach (var quest in questsData)
        {
            if (tracker.CompletedQuestIds.Contains(quest.Id))
            {
                quest.IsCompleted = true;
            }

            if (tracker.PinnedQuestIds.Contains(quest.Id))
            {
                quest.IsPinned = true;
            }

            var viewModel = new QuestViewModel(quest);
            viewModel.CompletionChanged += OnQuestCompletionChanged;
            viewModel.PinStatusChanged += OnQuestPinStatusChanged;
            _quests.Add(viewModel);
        }

        RefreshFilteredQuests();
    }

    private void RefreshFilteredQuests()
    {
        _filteredQuests.Clear();

        var filtered = _quests.AsEnumerable();

        if (HideCompleted)
        {
            filtered = filtered.Where(q => !q.IsCompleted);
        }

        if (OnlyKappa)
        {
            filtered = filtered.Where(q => q.Kappa);
        }

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var searchLower = SearchText.ToLower();
            filtered = filtered.Where(q => q.Name.ToLower().Contains(searchLower) || 
                                           q.ShortDescription.ToLower().Contains(searchLower));
        }

        foreach (var quest in filtered.OrderBy(q => q.Name))
        {
            _filteredQuests.Add(quest);
        }

        UpdateLabels();
    }

    private void UpdateLabels()
    {
        var totalQuests = _filteredQuests.Count;
        var completedQuests = _filteredQuests.Count(q => q.IsCompleted);
        QuestsLabel = $"Quests: {completedQuests}/{totalQuests}";

        var totalKappa = _quests.Count(q => q.Kappa);
        var completedKappa = _quests.Count(q => q.Kappa && q.IsCompleted);
        KappaLabel = $"Kappa: {completedKappa}/{totalKappa}";
    }

    private void ExecuteToggleCompletion(object? parameter)
    {
        if (parameter is QuestViewModel quest)
        {
            quest.IsCompleted = !quest.IsCompleted;
        }
    }

    private void ExecuteTogglePin(object? parameter)
    {
        if (parameter is QuestViewModel quest)
        {
            quest.IsPinned = !quest.IsPinned;
        }
    }

    private void OnQuestCompletionChanged(QuestViewModel quest)
    {
        SaveQuestProgress();
        RefreshFilteredQuests();
    }

    private void OnQuestPinStatusChanged(QuestViewModel quest)
    {
        SaveQuestProgress();
    }

    private void SaveQuestProgress()
    {
        var tracker = new QuestTracker();

        foreach (var quest in _quests)
        {
            if (quest.IsCompleted)
            {
                tracker.CompletedQuestIds.Add(quest.Quest.Id);
            }
            if (quest.IsPinned)
            {
                tracker.PinnedQuestIds.Add(quest.Quest.Id);
            }
        }

        _questService.SaveQuestProgress(tracker);
    }
}

public class QuestViewModel : ViewModelBase
{
    private Quest _quest;
    private bool _isCompleted;
    private bool _isPinned;

    public event Action<QuestViewModel>? CompletionChanged;
    public event Action<QuestViewModel>? PinStatusChanged;

    public Quest Quest => _quest;

    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            if (SetProperty(ref _isCompleted, value))
            {
                _quest.IsCompleted = value;
                CompletionChanged?.Invoke(this);
            }
        }
    }

    public bool IsPinned
    {
        get => _isPinned;
        set
        {
            if (SetProperty(ref _isPinned, value))
            {
                _quest.IsPinned = value;
                PinStatusChanged?.Invoke(this);
            }
        }
    }

    public string Name => _quest.Name;
    public string ShortDescription => _quest.ShortDescription;
    public bool Kappa => _quest.Kappa;

    public QuestViewModel(Quest quest)
    {
        _quest = quest;
        _isCompleted = quest.IsCompleted;
        _isPinned = quest.IsPinned;
    }
}
