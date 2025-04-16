using System.Text.Json;

namespace LabProject.Helpers
{
    public sealed class Utils
    {
        // Singleton örneği
        private static readonly Lazy<Utils> _instance = new(() => new Utils());
        public static Utils Instance => _instance.Value;

        // Private constructor -> dışarıdan new'lenemez
        private Utils() { }

        // Generic JSON export metodu
        public byte[] ExportToJson<T>(IEnumerable<T> data)
        {
            var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            return System.Text.Encoding.UTF8.GetBytes(json);
        }
    }
}
