# Biblioteca_Virtual



## Este segmento reúne os modelos de JSON utilizados para criação das entidades do sistema de biblioteca.

### 👤 Usuário — Criação
{
  "nome": "",
  "cpf": "",
  "email": "",
  "telefone": "00 (00) 00000-0000"
}

### ✍️ Autor — Criação
{
  "nome": "",
  "dataNascimento": "0001-01-01",
  "paisOrigem": "",
  "biografia": ""
}

### 📚 Livro — Criação
{
  "titulo": "",
  "anoPublicacao": 0,
  "isbn": "",
  "categoria": "",
  "quantidadeEstoque": 0,
  "autorId": 0
}

### 🔄 Empréstimo — Criação
{
  "usuarioId": 0,
  "livroId": 0
}

## Este segmento reune os EndPoints disponiveis.

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

### 🔄 Empréstimos (`/api/emprestimos`)
- **POST** `/` - Realiza um novo empréstimo.
- **PUT** `/{id}/devolver` - Realiza a devolução de um livro.
- **PUT** `/{id}/renovar` - Renova o prazo de um empréstimo.
- **GET** `/{usuarioId}` - Busca um emprestimo pelo ID.
- **GET** `/{usuarioId}/ativos` - Lista empréstimos ativos de um usuário.
- **GET** `/{usuarioId}/historico` - Lista histórico de empréstimos de um usuário.


