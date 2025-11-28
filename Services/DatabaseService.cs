using System.Net.Http.Json;
using GKFashionApp.Models;

namespace GKFashionApp.Services
{
    public class DatabaseService
    {
     
private string BaseUrl = "https://gk-3mkq.onrender.com/api/fashion";
       

        HttpClient _client;

        public DatabaseService()
        {
           
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _client = new HttpClient(handler);
           
            _client.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<List<FashionItem>> GetItemsAsync()
        {
            Console.WriteLine($"🔍 [APP LOG] Connecting to: {BaseUrl}");
            try 
            {
                var response = await _client.GetFromJsonAsync<List<FashionItem>>(BaseUrl);
                Console.WriteLine($"✅ [APP LOG] SUCCESS: Loaded {response?.Count} items");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [APP LOG] CONNECTION ERROR: {ex.Message}");
                // Agar emulator localhost access nahi kar pa raha
                if(ex.Message.Contains("connection refused"))
                    Console.WriteLine("💡 Tip: Check API Port or 'android:usesCleartextTraffic' in AndroidManifest.xml");
                
                return null; // Null return karenge taaki ViewModel ko pata chale error hai
            }
        }

        // ✅ Return type 'bool' hai taaki ViewModel me 'if (success)' chal sake
        public async Task<bool> SaveItemAsync(FashionItem item)
        {
            Console.WriteLine($"📤 [APP LOG] Saving Item: {item.Name}...");
            try
            {
                var response = await _client.PostAsJsonAsync(BaseUrl, item);
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ [APP LOG] SAVE SUCCESS!");
                    return true;
                }
                else
                {
                    Console.WriteLine($"⚠️ [APP LOG] SERVER ERROR: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [APP LOG] SAVE CRASH: {ex.Message}");
                // Error ViewModel tak phek rahe hain taaki Screen par 'Crash' popup aaye
                throw new Exception($"Connection Failed: {ex.Message}");
            }
        }

        public async Task DeleteItemAsync(FashionItem item)
        {
            try
            {
                await _client.PostAsync($"{BaseUrl}/delete/{item.Id}", null);
                Console.WriteLine("✅ [APP LOG] DELETED SUCCESS");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [APP LOG] DELETE ERROR: {ex.Message}");
            }
        }
    }
}