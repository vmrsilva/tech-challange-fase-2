# Tech Challenge FIAP

## Fase 2 - Arquitetura de Sistemas .NET

Nome: Valmir Severino da Silva <br/>
RM: 360650


## Executar projeto: <br/>
### Iniciar container Redis:<br/>
``docker run -d -p:6379:6379 redis:latest``

Acessar o diretório do projeto TechChallange.Api <br/> 
Executar o comando:
` dotnet run `

### Executar testes: <br/>
Acessar o diretório do projeto TechChallange.Test <br/>
Executar o comando:
`dotnet test`

### Executar monitoramento em localhost:<br/>

Posicionar no diretório: ../tech-challange-fase-2/TechChallange <br/>
e executar o comando para iniciar os containers:

```bash
docker-compose up --build -d
````

Comando parar os containers: <br/>
```bash
docker-compose down
```
