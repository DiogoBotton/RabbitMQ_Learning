using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.RabbitMQ.Producer.Inputs
{
    public class FileInput
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
