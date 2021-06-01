# RabbitMQ_Learning
Repositório com fins de estudo utilizando RabbitMQ (Software de mensagens)

**Instalação de Erlang (Linguaguem que foi escrito o RabbitMQ, necessário para funcionar)**
* [Download Erlang](https://www.erlang.org/downloads)

**Instalação RabbitMQ**
* [Download RabbitMQ](https://www.rabbitmq.com/download.html)

* Após a instalação dos dois, abra o Command Prompt do Rabbit MQ
* Digite o seguinte comando para liberar alguns plugins uteis para o gerenciamento das mensagens com uma interface gráfica

### `rabbitmq-plugins enable rabbitmq_management`

* Com todos estes processos feitos, o servidor de RabbitMQ estará rodando em sua maquina
* Porta do servidor: 5672
* Porta da interface gráfica do RabbitMQ: 15672

**Acessar interface gráfica**
* No navegador, coloque na URL o seguinte endereço

### `http://localhost:15672`

* No início irá ser preciso colocar login e senha, apenas digite **guest** nos campos de usuário e senha