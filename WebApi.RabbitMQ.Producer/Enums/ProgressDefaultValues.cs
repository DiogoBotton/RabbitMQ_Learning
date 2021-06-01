using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.RabbitMQ.Producer.Enums
{
    public static class ProgressDefaultValues
    {
        public static string GetValue(EnumProgress progress)
        {
            string retorno = "";

            switch (progress)
            {
                case EnumProgress.Reading:
                    retorno = "Lendo arquivo ";
                    break;
                case EnumProgress.ReadingComplete:
                    retorno = "Leitura do arquivo completa";
                    break;
                case EnumProgress.Processing:
                    retorno = "Processando conteúdo do arquivo...";
                    break;
                case EnumProgress.ProcessingComplete:
                    retorno = "Processamento concluído";
                    break;
                case EnumProgress.ERROR:
                    retorno = "ERRO";
                    break;
                default:
                    break;
            }

            return retorno;
        }
    }

    public enum EnumProgress
    {
        Reading = 1,
        ReadingComplete = 2,
        Processing = 3,
        ProcessingComplete = 4,
        ERROR = 5
    }
}
