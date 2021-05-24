using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Notification.Domains
{
    public class Progress
    {
        public string NomeArquivo { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
