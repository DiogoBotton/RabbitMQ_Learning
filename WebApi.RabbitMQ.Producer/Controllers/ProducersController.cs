using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApi.RabbitMQ.Producer.Domains;
using WebApi.RabbitMQ.Producer.Inputs;
using WebApi.RabbitMQ.Producer.Producers;

namespace WebApi.RabbitMQ.Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducersController : ControllerBase
    {
        private ILogger<ProducersController> _logger { get; set; }


        public ProducersController(ILogger<ProducersController> logger)
        {
            _logger = logger;
        }

        [HttpPost("new")]
        public async Task<IActionResult> Post([FromForm] FileInput input)
        {
            try
            {
                List<Pessoa> result = new List<Pessoa>();
                Progress currentProgress = new Progress(input.File.FileName);

                try
                {
                    // OpenReadStream para ler o CSV
                    using (var reader = new StreamReader(input.File.OpenReadStream()))
                    {
                        // Envia Status de progresso para fila
                        Messenger.SendProgressQeue(currentProgress);

                        // Enquanto houver linhas, Adiciona-as na lista
                        while (reader.Peek() >= 0)
                        {
                            string line = await reader.ReadLineAsync();

                            string[] itens = line.Split(",");

                            string nome = itens[0].Replace("\"", "");
                            //int idade = Convert.ToInt32(itens[1].Replace("\"", ""));

                            Pessoa p = new Pessoa(nome, 20);

                            result.Add(p);
                        }

                        currentProgress.UpdateStatusComplete();
                        Messenger.SendProgressQeue(currentProgress);
                    }
                }
                catch (Exception ex)
                {
                    currentProgress.UpdateStatusError("Houve algum erro na leitura do arquivo. ErrorMessage: " + ex.Message);

                    // Envia Status de Erro para fila
                    Messenger.SendProgressQeue(currentProgress);

                    _logger.LogError("Erro ler linha do arquivo.", ex);
                    return StatusCode((int)HttpStatusCode.InternalServerError, ex);
                }

                Messenger.SendPessoasQeue(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro ao adicionar itens na fila.", ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, ex);
            }
        }
    }
}
