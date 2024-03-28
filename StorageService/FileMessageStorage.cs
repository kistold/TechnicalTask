using Shared.Contract;
using System.Text;

namespace StorageService
{
    public sealed class FileMessageStorage : IMessageStorage<PageVisitedEvent>
    {
        private readonly string _filePath;

        public FileMessageStorage(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            fileInfo.Directory?.Create();

            _filePath = filePath;
        }

        public void Store(PageVisitedEvent message)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(message.VisitDateTime.ToString("o"));
            stringBuilder.Append('|');
            stringBuilder.Append(string.IsNullOrEmpty(message.Referer) ? "null" : message.Referer);
            stringBuilder.Append('|');
            stringBuilder.Append(string.IsNullOrEmpty(message.UserAgent) ? "null" : message.UserAgent);
            stringBuilder.Append('|');
            stringBuilder.Append(message.VisitorIp);

            using var streamWriter = File.AppendText(_filePath);
            streamWriter.WriteLine(stringBuilder.ToString());
        }
    }
}
