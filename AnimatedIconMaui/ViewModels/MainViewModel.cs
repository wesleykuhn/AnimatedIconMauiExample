using System.Windows.Input;

namespace AnimatedIconMaui.ViewModels;

public class MainViewModel : BaseViewModel
{
    private bool isDownloading;
    public bool IsDownloading
    {
        get => isDownloading;
        set => SetProperty(ref isDownloading, value);
    }

    private bool downloaded;
    public bool Downloaded
    {
        get => downloaded;
        set => SetProperty(ref downloaded, value);
    }

    private ICommand downloadCommand;
    public ICommand DownloadCommand => downloadCommand ??= new Command(async () => await DownloadCommandExecute());

    private async Task DownloadCommandExecute()
    {
        //Ready to download
        IsDownloading = true;

        //Doing download...
        await Task.Delay(10000);

        //After 4 seconds, it completes
        Downloaded = true;
    }
}
