using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.RabbitMQ.Producer.Enums
{
    public enum EnumProgress
    {
        PROCESSING = 1,
        COMPLETE = 2,
        ERROR = 3
    }
}
