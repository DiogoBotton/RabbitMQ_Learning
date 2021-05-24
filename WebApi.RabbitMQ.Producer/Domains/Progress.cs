using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.RabbitMQ.Producer.Enums;

namespace WebApi.RabbitMQ.Producer.Domains
{
    public class Progress
    {
        public string NomeArquivo { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }

        public Progress(string nomeArquivo)
        {
            NomeArquivo = nomeArquivo;
            Status = EnumProgress.PROCESSING.ToString();
        }

        public void UpdateStatusComplete()
        {
            this.Status = EnumProgress.COMPLETE.ToString();
        }

        public void UpdateStatusError(string errorMessage)
        {
            this.Status = EnumProgress.ERROR.ToString();
            this.Message = errorMessage;
        }
    }
}
