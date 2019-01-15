using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VirtualCashCard.Interface;

namespace VirtualCashCard.Core
{
    public class FileReader : IDataContext
    {
        ILogger<FileReader> _logger = null;
        EventWaitHandle _waitHandle = new EventWaitHandle(true, EventResetMode.AutoReset, "CROSS_PROCESS_ACCESS");
        
        public FileReader(ILogger<FileReader> logger)
        {
            _logger = logger;
        }

        public async Task<string> ProcessRead(string fileName)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            if (!File.Exists(filePath))
            {
                _logger.LogInformation($"File not found: {filePath}");
            }
            else
            {
                try
                {
                    string text = await ReadTextAsync(filePath);
                    return text;
                }
                catch (Exception ex)
                {
                    _logger.LogInformation($"Error: {ex}");
                }
            }
            return string.Empty;
        }

        private async Task<string> ReadTextAsync(string filePath)
        {
            using (FileStream sourceStream = new FileStream(filePath,
                FileMode.Open, FileAccess.Read, FileShare.Read,
                bufferSize: 4096, useAsync: true))
            {
                StringBuilder sb = new StringBuilder();
                StreamReader s1 = new StreamReader(sourceStream);
                char[] buffer = new char[128];
                int numRead;

                while ((numRead = await s1.ReadAsync(buffer, 0, buffer.Length)) != 0)
                {
                    string text = new string(buffer, 0, numRead);
                    sb.Append(text);
                }
                return sb.ToString();
            }
        }

        public async void ProcessWrite(string content, string fileName)
        {
            try
            {
                _waitHandle.WaitOne();
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

                using (FileStream sourceStream = new FileStream(filePath,
                    FileMode.Truncate, FileAccess.Write, FileShare.None,
                    bufferSize: 4096, useAsync: true))
                {
                    using (StreamWriter writer = new StreamWriter(sourceStream))
                    {
                        await writer.WriteAsync(content);
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Error: {ex}");
            }
            finally
            {
                _waitHandle.Set();
            }
        }
    }
}
