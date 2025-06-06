using currency.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace currency.Services
{
    public class CurrencyApiService
    {
        //api.frankfurter.app/latest?to=KRW&from=JPY&amount=100

        private readonly HttpClient _httpClient = new();

        private const string BaseCurrency = "KRW";
        //private static readonly string[] TargetCurrencies = { "USD", "EUR" };
        private static readonly string[] TargetCurrencies = { "USD", "EUR", "GBP", "AUD", "CAD", "CHF", "CNY", "HKD", "NZD" };
        private const string BaseUrl = "https://api.frankfurter";
        private readonly Dictionary<string, string> _currencyNames = new Dictionary<string, string>
        {
            {"KRW", "한국 원화"},
            {"USD", "미국 달러"},
            {"EUR", "유로"}, 
            {"JPY", "일본 엔"},
            {"GBP", "영국 파운드"},
            {"AUD", "호주 달러"}, 
            {"CAD", "캐나다 달러"}, 
            {"CHF", "스위스 프랑"}, 
            {"CNY", "중국 위안"}, 
            {"HKD", "홍콩 달러"},
            {"NZD", "뉴질랜드 달러"}
    
        };

        //public async Task<List<Currency>> GetConvertedRatesAsync()
        //{
        //var currencies = new List<Currency>();
        //using var httpClient = new HttpClient();

        //string yesterday = DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd");
        //var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };



        //foreach (var target in TargetCurrencies)
        //{
        //    var url = $"{BaseUrl}?to={BaseCurrency}&from={target}";
        //    //api.frankfurter.app/latest?to=KRW&from=USD
        //    Debug.WriteLine(url);

        //    try
        //    {
        //        var response = await _httpClient.GetAsync(url);
        //        response.EnsureSuccessStatusCode();
        //        //Debug.WriteLine(response);

        //        var json = await response.Content.ReadAsStringAsync();
        //        Debug.WriteLine(json);




        //        var result = JsonSerializer.Deserialize<ExchangeResponse>(json, options);

        //        if (result != null && result.Rates != null)
        //        {
        //            if (result.Rates.TryGetValue(BaseCurrency, out double rate))
        //            {
        //                currencies.Add(new Currency
        //                {
        //                    Code = target,
        //                    Rate = rate,
        //                    PreviousRate = 0,
        //                    LastUpdate = DateTime.Now
        //                });
        //            }
        //            else
        //            {
        //                Debug.WriteLine($"Rates에 '{BaseCurrency}' 키가 존재하지 않음");
        //            }
        //        }
        //    }
        public async Task<List<Currency>> GetConvertedRatesAsync()
        {
            var currencies = new List<Currency>();
            using var httpClient = new HttpClient();

            string yesterday = DateTime.Today.AddDays(-2).ToString("yyyy-MM-dd");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            foreach (var target in TargetCurrencies)
            {
                try
                {
                    // 오늘 환율
                    var todayUrl = $"{BaseUrl}.app/latest?from={target}&to={BaseCurrency}";
                    var todayJson = await httpClient.GetStringAsync(todayUrl);
                    var todayData = JsonSerializer.Deserialize<ExchangeResponse>(todayJson, options);
                    var todayRate = todayData?.Rates[BaseCurrency] ?? 0;

                    // 어제 환율
                    var yesterdayUrl = $"{BaseUrl}.dev/v1/{yesterday}?from={target}&to={BaseCurrency}";
                    Debug.WriteLine(yesterdayUrl);
                   var yesterdayJson = await httpClient.GetStringAsync(yesterdayUrl);
                    var yesterdayData = JsonSerializer.Deserialize<ExchangeResponse>(yesterdayJson, options);
                    var yesterdayRate = yesterdayData?.Rates[BaseCurrency] ?? 0;

                    currencies.Add(new Currency
                    {
                        Code = target,
                        Name = _currencyNames.TryGetValue(target, out var name) ? name : target,
                        Rate = todayRate,
                        PreviousRate = yesterdayRate,
                        LastUpdate = DateTime.Now
                    });
                    var c = currencies[^1]; // 마지막에 추가된 항목
                    Debug.WriteLine($"{c.Code}: {c.Rate} (어제: {c.PreviousRate}) | 변화율: {c.RateChangePercent:F2}% {c.ChangeIcon}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"오류 발생: {target} → {BaseCurrency} 변환 실패\n{ex.Message}");
                }
            }


            return currencies;
        }


    }
}

//변동률 확인
//private const string BaseUrl = "/live";
//public async Task<List<Currency>> GetLatestRatesAsync()
//{
//    var currencies = new List<Currency>();
//    var symbols = string.Join(",", TargetCurrencies);
//    var url = $"{BaseUrl}?access_key={AccessKey}&source={BaseCurrency}&currencies={symbols}";
//    //Console.WriteLine(url);
//   // Debug.WriteLine(url);
//    try
//    {
//        var response = await _httpClient.GetAsync(url);
//        response.EnsureSuccessStatusCode();
//       // Debug.WriteLine(response);
//         var json = await response.Content.ReadAsStringAsync();
//           var data = JsonSerializer.Deserialize<LiveRateResponse>(json);

//       // Debug.WriteLine(data);

//        if (data?.Quotes != null)
//        {
//            var now = DateTime.Now;
//            foreach (var kvp in data.Quotes)
//            {
//                // "KRWUSD" → "USD" 로 분리
//                var code = kvp.Key.Replace(BaseCurrency, "");

//                currencies.Add(new Currency
//                {
//                    Code = code,
//                    Name = code, // 이름은 API에서 제공되지 않으므로 코드로 대체
//                    Rate = kvp.Value,
//                    PreviousRate = 0, // 이전 데이터가 없으면 0
//                    LastUpdate = DateTime.Now
//                });
//            }
//        }
//        else
//        {
//        Console.WriteLine("API 응답 실패 또는 데이터 없음");
//    }
//}
//catch (Exception ex)
//{
//    Console.WriteLine($"API 요청 오류: {ex.Message}");
//}


