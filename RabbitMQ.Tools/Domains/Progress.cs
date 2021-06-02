using RabbitMQ.Tools.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Tools.Domains
{
    public class Progress
    {
        public string NomeArquivo { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }

        // Construtor com status Reading
        public Progress(string nomeArquivo)
        {
            NomeArquivo = nomeArquivo;
            Status = EnumProgress.Reading.ToString();
            Message = ProgressDefaultValues.GetValue(EnumProgress.Reading) + $"{nomeArquivo}...";
        }

        // Construtor com status Processing
        public Progress()
        {
            NomeArquivo = "Sem Referência";
            Status = EnumProgress.Processing.ToString();
            Message = ProgressDefaultValues.GetValue(EnumProgress.Processing);
        }

        public void UpdateStatusProcessingComplete()
        {
            Status = EnumProgress.ProcessingComplete.ToString();
            Message = ProgressDefaultValues.GetValue(EnumProgress.ProcessingComplete);
        }

        public void UpdateStatusReadingComplete()
        {
            Status = EnumProgress.ReadingComplete.ToString();
            Message = ProgressDefaultValues.GetValue(EnumProgress.ReadingComplete);
        }

        public void UpdateStatusError(string errorMessage)
        {
            Status = EnumProgress.ERROR.ToString();
            Message = errorMessage;
        }
    }
}
