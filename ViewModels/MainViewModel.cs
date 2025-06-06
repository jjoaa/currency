using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using currency.Helpers;
using System.Windows.Input;
using currency.Models;
using currency.Services;
using System.Diagnostics;

//namespace currency.ViewModels
//{
//   public class MainViewModel : INotifyPropertyChanged
// {
//private readonly CurrencyApiService _service = new();

//public ICommand LoadRatesCommand { get; }

//public ObservableCollection<Currency> Currencies { get; } = new();

//public MainViewModel()
//{
//    LoadRatesCommand = new RelayCommand(async () => await LoadCurrenciesAsync());
//}

//public async Task LoadCurrenciesAsync()
//{
//    // Debug.WriteLine("환율 데이터 로드 시작");

//    var result = await _service.GetLatestRatesAsync();
//    //Debug.WriteLine("불러온 통화 수: " + result.Count);
//    //Console.WriteLine("불러온 통화 수: " + result.Count);
//    Currencies.Clear();
//    foreach (var currency in result)
//    {
//        Console.WriteLine($"[{currency.Code}] {currency.Rate}");
//        Currencies.Add(currency);
//    }
//}

//public event PropertyChangedEventHandler PropertyChanged;
//protected void OnPropertyChanged([CallerMemberName] string name = "") =>
//    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
//    }
//}


namespace currency.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly CurrencyApiService _apiService = new();

        public ObservableCollection<Currency> Currencies { get; set; } = new();

        public ICommand LoadRatesCommand { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public MainViewModel()
        {
            LoadRatesCommand = new RelayCommand(LoadRatesAsync, () => !IsLoading);
        }

        private async Task LoadRatesAsync()
        {
            if (IsLoading) return;

            IsLoading = true;

            try
            {
                Debug.WriteLine("환율 데이터 로드 시작");

                var latestRates = await _apiService.GetConvertedRatesAsync();

                Currencies.Clear();
                foreach (var currency in latestRates)
                {
                    Currencies.Add(currency);
                }

                Debug.WriteLine($"불러온 통화 수: {Currencies.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"데이터 로드 실패: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}