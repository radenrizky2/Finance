using Newtonsoft.Json;

namespace Playground.Identity.BLL
{
    public static class FileManager
    {
        public static T LoadDataFromFile<T>(string folderFilePath)
        {
            string path = Path.Combine(Environment.CurrentDirectory, folderFilePath);
            T result = default;
            using (var reader = new StreamReader(path))
            {
                var data = reader.ReadToEnd();
                result = JsonConvert.DeserializeObject<T>(data);
            }
            return result;
        }

        public static T LoadDataFromFilePath<T>(string filePath)
        {
            T result = default;
            using (var reader = new StreamReader(filePath))
            {
                var data = reader.ReadToEnd();
                result = JsonConvert.DeserializeObject<T>(data);
            }
            return result;
        }
    }
}
