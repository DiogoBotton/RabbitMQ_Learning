using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Consumer.Domains
{
    public class Pessoa
    {
        public string Nome { get; set; }
        public int Idade { get; set; }
        public string Genero { get; set; }

        public Pessoa(string nome, int idade, string genero)
        {
            Nome = nome;
            Idade = idade;
            Genero = genero;
        }
    }
}
