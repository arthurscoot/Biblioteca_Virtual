# Biblioteca Virtual API

API RESTful para gerenciamento de uma biblioteca virtual, desenvolvida em .NET 8.

## 🚀 Instruções de Setup

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) 

### Configuração
1. Clone o repositório:
   ```bash
   git clone https://github.com/arthurscoot/Biblioteca_Virtual.git
   ```

2. Configure a string de conexão no `appsettings.json` (localizado em `Library`) ou via variáveis de ambiente. O padrão espera uma conexão local:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=LibraryDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```

3. Restaure as dependências:
   ```bash
   dotnet restore
   ```

##  Como executar Migrations

<<<<<<< HEAD
O projeto utiliza Entity Framework Core Code-First. Para aplicar as migrações e criar o banco de dados:

1. Navegue até a pasta do projeto principal:
   ```bash
   cd .\Library\
   ```

2. Execute o comando de atualização do banco:
   ```bash
   dotnet ef database update
   ```

## 🧪 Como rodar Testes

O projeto possui testes unitários cobrindo Serviços e Controllers.

1. Navegue até a raiz da solução. (`Library.Tests`)
Se estiver em `Library`faça:   
```bash
cd ..
cd .\Library.Tests\
 ```
2. Execute os testes:
   ```bash
   dotnet test
   ```

## 🛠 Decisões Técnicas

*   **Arquitetura**: O projeto segue princípios de **Clean Architecture** e **DDD (Domain-Driven Design)**, organizando o código em camadas lógicas (Domain, Application, Infrastructure, API) para separação de responsabilidades.
*   **Entity Framework Core**: Utilizado como ORM para mapeamento objeto-relacional e interação com o SQL Server.
*   **Repository Pattern**: Implementado para abstrair a lógica de acesso a dados e facilitar a testabilidade da camada de aplicação.
*   **Domain Validations**: As regras de negócio e validações essenciais (ex: idade mínima, validação de datas, regras de empréstimo) estão encapsuladas nas Entidades de Domínio, garantindo integridade.
*   **TimeProvider**: Utilização da abstração `TimeProvider` (nativa do .NET 8) para manipulação de datas. Isso permite testes unitários determinísticos simulando passagem de tempo para cálculo de multas e prazos.
*   **Testes Unitários**: Implementados com **xUnit** e **Moq**, garantindo a qualidade do código e prevenindo regressões.

---

## 📖 Endpoints da API
=======
## Este segmento reúne os EndPoints disponiveis.
>>>>>>> c6ef4bad76a220cb416a586ddede5bd0366b704e

### 👤 Usuários (`/api/usuarios`)
- **GET** `/` - Lista os usuários ativos.
- **GET** `/{cpf}` - Busca um usuário pelo CPF.
- **POST** `/` - Cria um novo usuário.
- **PUT** `/{id}` - Atualiza os dados de um usuário.
- **DELETE** `/{id}` - Desativa um usuário.

### ✍️ Autores (`/api/autores`)
- **GET** `?page=1&size=10` - Lista autores com paginação.
- **GET** `/{id}` - Busca um autor pelo ID.
- **POST** `/` - Cria um novo autor.
- **PUT** `/{id}` - Atualiza um autor.
- **DELETE** `/{id}` - Desativa um autor.

### 📚 Livros (`/api/livros`)
- **GET** `?titulo={titulo}&isbn={isbn}` - Pesquisa livros por título ou ISBN.
- **GET** `/{id}` - Busca um livro pelo ID.
- **GET** `/autores/{autorId}` - Lista livros de um autor pelo ID do autor.
- **GET** `/estoque` - Lista livros disponíveis em estoque.
- **POST** `/` - Cadastra um novo livro.
- **PUT** `/{id}` - Atualiza um livro.
- **DELETE** `/{id}` - Deleta um livro.

### 🔄 Empréstimos (`/api/emprestimos`)
- **POST** `/` - Realiza um novo empréstimo.
- **PUT** `/{id}/devolver` - Realiza a devolução de um livro.
- **PUT** `/{id}/renovar` - Renova o prazo de um empréstimo.
- **GET** `/{id}` - Busca um emprestimo pelo ID.
- **GET** `/{usuarioId}/ativos` - Lista empréstimos ativos de um usuário.
- **GET** `/{usuarioId}/historico` - Lista histórico de empréstimos de um usuário.

### 🧾 Relatórios (`/api/relatorios`)
- **GET** `/multas_pendentes` - Calcula o total a receber de multas pendentes.
- **GET** `/usuarios_atrasados` - Lista usuários com empréstimos atrasados.

### 📊 Estatísticas (`/api/estatisticas`)
- **GET** `/top_livros` - Retorna uma lista de livros mais emprestados.
- **GET** `/top_autores` - Retorna uma lista de autores mais emprestados.
<<<<<<< HEAD

## Modelos JSON (Exemplos)

### 👤 Usuário — Criação
```json
{
  "nome": "João Silva",
  "cpf": "12345678900",
  "cpfResponsavel": "",
  "email": "joao@email.com",
  "telefone": "55 (21) 98701-8732",
  "dataNascimento": "2000-01-01"
}
```
Usuário menor de 16 anos:

```json
{
  "nome": "Pedro Santos",
  "cpf": "22345678900",
  "cpfResponsavel": "00345678901",
  "email": "jorge@gmail.com",
  "telefone": "55 (11) 98131-8732",
  "dataNascimento": "2015-01-01"
}
```

### ✍️ Autor — Criação
```json
{
  "nome": "Arthur Moreira",
  "dataNascimento": "2005-10-11",
  "paisOrigem": "Brasil",
  "biografia": "Autor de Mar Púrpura"
}
```

### 📚 Livro — Criação
```json
{
  "titulo": "Mar Púrpura",
  "anoPublicacao": 2026,
  "isbn": "9788700631625",
  "categoria": "Terror",
  "quantidadeEstoque": 100,
  "autorId": 1
}
```

### 🔄 Empréstimo — Criação
```json
{
  "usuarioId": 1,
  "livroId": 1
}
```
=======
>>>>>>> c6ef4bad76a220cb416a586ddede5bd0366b704e
