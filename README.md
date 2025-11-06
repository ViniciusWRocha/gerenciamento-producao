# Sistema de Gerenciamento de Produção (SIGE)

Sistema completo para gerenciamento de produção de obras, caixilhos, produção e metas mensais. Desenvolvido em ASP.NET Core MVC com integração ao Google Calendar.

## 📋 Índice

- [Visão Geral](#visão-geral)
- [Funcionalidades](#funcionalidades)
- [Tecnologias](#tecnologias)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Pré-requisitos](#pré-requisitos)
- [Instalação](#instalação)
- [Configuração](#configuração)
- [Uso](#uso)
- [API](#api)
- [Estrutura de Dados](#estrutura-de-dados)

## 🎯 Visão Geral

O SIGE é um sistema web que centraliza o acompanhamento de:
- **Obras**: Gestão completa de obras com status, prioridades, bandeiras e integração com Google Calendar
- **Caixilhos**: Controle de produção e liberação de caixilhos vinculados a obras
- **Produção**: Registro de produções diárias com vínculo a famílias de caixilho e usuários
- **Metas Mensais**: Acompanhamento de objetivos de peso (kg) por mês/ano
- **Usuários**: Sistema de autenticação com diferentes perfis (Administrador, Gerente, Outros)
- **Relatórios**: Geração de relatórios consolidados de produção

## ✨ Funcionalidades

### Dashboard
- Indicadores consolidados (obras, produções, caixilhos, usuários ativos)
- Gráficos de evolução mensal
- Distribuição por bandeira das obras
- Ranking de usuários
- Status das metas
- Alertas automáticos

### Gestão de Obras
- Cadastro completo de obras com dados cadastrais
- Status de acompanhamento (Planejada, Em Andamento, Concluída, Atrasada)
- Sistema de prioridades e bandeiras
- Cálculo automático de peso produzido e percentual de conclusão
- Integração automática com Google Calendar
- Importação de múltiplos arquivos XML

### Gerenciamento de Caixilhos
- Catálogo completo de caixilhos
- Liberação individual ou em lote por família
- Vinculação com obras, famílias e tipos
- Controle de dimensões, quantidade e peso

### Produção
- Registro de produções diárias
- Vinculação com usuários e famílias de caixilho
- Controle de status de produção e liberação

### Metas Mensais
- Definição de metas de peso (kg) por mês/ano
- Acompanhamento de progresso
- Alertas de metas em risco

### Autenticação
- Sistema de login com cookies
- Perfis de acesso diferenciados:
  - **Administrador**: Acesso completo
  - **Gerente**: Gestão operacional
  - **Outros**: Visualização e execução

## 🛠️ Tecnologias

- **.NET 9.0**
- **ASP.NET Core MVC**
- **Entity Framework Core 9.0.9**
- **SQL Server**
- **Google Calendar API**
- **Bootstrap** (via lib)
- **jQuery** (via lib)

## 📁 Estrutura do Projeto

```
gerenciamento-producao/
├── GerenciamentoProducao/          # Aplicação MVC principal
│   ├── Controllers/                 # Controladores MVC
│   ├── Models/                      # Modelos de dados
│   ├── Views/                       # Views Razor
│   ├── Repositories/                # Repositórios (padrão Repository)
│   ├── Interfaces/                  # Interfaces dos repositórios
│   ├── Services/                    # Serviços (Google Calendar)
│   ├── ViewModel/                   # ViewModels
│   ├── Data/                        # DbContext e Factory
│   ├── Migrations/                  # Migrações do EF Core
│   └── wwwroot/                     # Arquivos estáticos
│
└── GerenciamentoProducao.API/       # API REST
    ├── Controllers/                 # Controladores da API
    ├── DTOs/                        # Data Transfer Objects
    └── Services/                    # Serviços da API
```

## 📋 Pré-requisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) ou SQL Server LocalDB
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)
- Conta Google para integração com Google Calendar
- Credenciais do Google Calendar API (arquivo `credentials-service.json`)

## 🚀 Instalação

1. **Clone o repositório**
```bash
git clone <url-do-repositorio>
cd gerenciamento-producao
```

2. **Restore as dependências**
```bash
dotnet restore
```

3. **Configure o banco de dados**
   - Edite `appsettings.json` com sua string de conexão
   - Execute as migrações:
```bash
cd GerenciamentoProducao
dotnet ef database update
```

4. **Configure as credenciais do Google Calendar**
   - Adicione o arquivo `credentials-service.json` na pasta `Credentials/`
   - Configure a chave do Google Calendar no `appsettings.json`

## ⚙️ Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GereciadorDeProducaoDB;Trusted_Connection=true;MultipleActiveResultSets=true;"
  },
  "Google": {
    "key": "sua-chave-do-calendario@group.calendar.google.com"
  }
}
```

### Credenciais do Google Calendar

1. Acesse o [Google Cloud Console](https://console.cloud.google.com/)
2. Crie um projeto ou selecione um existente
3. Ative a API do Google Calendar
4. Crie credenciais de conta de serviço
5. Baixe o arquivo JSON e renomeie para `credentials-service.json`
6. Coloque o arquivo em `GerenciamentoProducao/Credentials/`

## 💻 Uso

### Executar a aplicação MVC

```bash
cd GerenciamentoProducao
dotnet run
```

Acesse: `https://localhost:5001` ou `http://localhost:5000`

### Executar a API

```bash
cd GerenciamentoProducao.API
dotnet run
```

Acesse a documentação Swagger: `https://localhost:5001/swagger` (em ambiente de desenvolvimento)

## 🔌 API

A API REST oferece endpoints para:

- **Obras**: CRUD completo de obras
- **Caixilhos**: Gerenciamento de caixilhos
- **Integração com Google Calendar**: Sincronização de eventos

### Documentação Swagger

Quando executada em ambiente de desenvolvimento, a API expõe documentação Swagger em `/swagger`.

## 📊 Estrutura de Dados

### Principais Entidades

- **Usuario**: Usuários do sistema com diferentes tipos/perfis
- **Obra**: Obras em produção com status, prioridades e bandeiras
- **Caixilho**: Caixilhos vinculados a obras, famílias e tipos
- **Producao**: Registros de produção diária
- **FamiliaCaixilho**: Agrupamento de caixilhos
- **TipoCaixilho**: Tipos/categorias de caixilhos
- **TipoUsuario**: Tipos de usuários (Administrador, Gerente, etc.)

### Relacionamentos

- Obra → Usuario (Responsável)
- Caixilho → Obra, FamiliaCaixilho, TipoCaixilho
- Producao → Usuario, FamiliaCaixilho
- Usuario → TipoUsuario

## 📝 Migrações

Para criar uma nova migração:

```bash
cd GerenciamentoProducao
dotnet ef migrations add NomeDaMigracao
```

Para aplicar migrações:

```bash
dotnet ef database update
```

## 🔐 Autenticação

O sistema utiliza autenticação por cookies com:
- Login via `/Usuario/Login`
- Sessão com expiração de 30 minutos
- Sliding expiration habilitado
- Redirecionamento para `/Usuario/AcessoNegado` em caso de acesso negado

## 📖 Documentação Adicional

Consulte o arquivo `manual-usuario.txt` na pasta `GerenciamentoProducao/` para documentação detalhada do sistema.

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está sob licença proprietária.

## 👥 Autores

Desenvolvido para gerenciamento de produção.

## 📞 Suporte

Para suporte e dúvidas:
- Entre em contato com a equipe de TI responsável pelo SIGE
- Para questões de dados operacionais, comunique-se com o gerente de produção

