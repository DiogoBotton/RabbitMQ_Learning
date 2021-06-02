using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.RabbitMQ.Producer.Enums
{
    public class QueuesDefaultValues
    {
        public static string GetValue(EnumQueues progress)
        {
            string retorno = "";

            switch (progress)
            {
                case EnumQueues.ProgressQueue:
                    retorno = "progressQueue";
                    break;
                case EnumQueues.PessoasQueue:
                    retorno = "pessoasQueue";
                    break;
            }

            return retorno;
        }
    }

    public enum EnumQueues
    {
        ProgressQueue,
        PessoasQueue,
    }
}

