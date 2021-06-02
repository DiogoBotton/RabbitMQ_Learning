using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Tools.Domains;
using RabbitMQ.Tools.Enums;
using RabbitMQ.Tools.Producers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApi.RabbitMQ.Producer.Inputs;

namespace WebApi.RabbitMQ.Producer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProducersController : ControllerBase
    {
        private ILogger<ProducersController> _logger { get; set; }
        private IMessenger _messenger { get; set; }


        public ProducersController(ILogger<ProducersController> logger, IMessenger messenger)
        {
            _logger = logger;
            _messenger = messenger;
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
                        _messenger.SendMessage(currentProgress, QueuesDefaultValues.GetValue(EnumQueues.ProgressQueue));

                        int cont = 0;
                        // Enquanto houver linhas, Adiciona-as na lista
                        while (reader.Peek() >= 0)
                        {
                            string line = await reader.ReadLineAsync();

                            // Caso a linha atual seja o cabeçalho, pula o processo
                            if (cont == 0)
                            {
                                cont++;
                                continue;
                            }

                            string[] itens = line.Split(",");

                            string nome = itens[0].Replace("\"", "");
                            int idade = Convert.ToInt32(itens[3].Replace("\"", ""));
                            string genero = itens[4].Replace("\"", "");

                            Pessoa p = new Pessoa(nome, idade, genero);

                            result.Add(p);
                        }

                        currentProgress.UpdateStatusReadingComplete();
                        _messenger.SendMessage(currentProgress, QueuesDefaultValues.GetValue(EnumQueues.ProgressQueue));
                    }
                }
                catch (Exception ex)
                {
                    currentProgress.UpdateStatusError("Houve algum erro na leitura do arquivo. ErrorMessage: " + ex.Message);

                    // Envia Status de Erro para fila
                    _messenger.SendMessage(currentProgress, QueuesDefaultValues.GetValue(EnumQueues.ProgressQueue));

                    _logger.LogError("Erro ao ler linha do arquivo.", ex);
                    return StatusCode((int)HttpStatusCode.InternalServerError, ex);
                }

                // Envia resultado para fila pessoasQueue
                _messenger.SendMessage(result, QueuesDefaultValues.GetValue(EnumQueues.PessoasQueue));

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
