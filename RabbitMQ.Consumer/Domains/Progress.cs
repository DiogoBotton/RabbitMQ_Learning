using RabbitMQ.Consumer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Domains
{
    public class Progress
    {
        public string NomeArquivo { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }

        public Progress()
        {
            NomeArquivo = "Sem referência";
            Status = EnumProgress.Processing.ToString();
            Message = ProgressDefaultValues.GetValue(EnumProgress.Processing);
        }

        public void UpdateStatusComplete()
        {
            this.Status = EnumProgress.ProcessingComplete.ToString();
            Message = ProgressDefaultValues.GetValue(EnumProgress.ProcessingComplete);
        }

        public void UpdateStatusError(string errorMessage)
        {
            this.Status = EnumProgress.ERROR.ToString();
            this.Message = errorMessage;
        }
    }
}
