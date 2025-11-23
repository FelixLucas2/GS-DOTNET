# SkillUp API – Plataforma de Requalificação Profissional com IA

# Membros 
  Gabriel Duarte - RM556972 - 2TDSPG 
  Gabriel Yuji Suzuki - RM556588 - 2TDSPZ 
  Lucas Felix - RM97677 - 2TDSR

## Visão Geral  
A **SkillUp API** é uma aplicação web RESTful desenvolvida em .NET, que faz parte da plataforma “Plataforma de Requalificação Profissional com IA”.  
Ela visa ajudar profissionais a se adaptarem ao futuro do trabalho — combinando recomendações de cursos, análise de perfil, e conexão com habilidades emergentes no mercado.  
Alinhada aos ODS 4 (Educação de Qualidade) e ODS 8 (Trabalho Decente e Crescimento Econômico).

## Funcionalidades Principais  
- Cadastro, listagem, atualização e remoção de **usuários**.  
- Cadastro, listagem, atualização e remoção de **cursos**.  
- Associação entre usuários e cursos (inscrição, remoção).  
- Geração de **recomendações de cursos** para usuário específico.  
- Autenticação via JWT e/ou uso de API Key para acesso aos endpoints protegidos.  
- Documentação da API via Swagger / OpenAPI.  
- Versionamento de API (ex: `/api/v1/...`).  
- Health Checks, logging (Serilog) e monitoramento.

## Tecnologias Utilizadas  
- .NET 8.0 (C#)  
- ASP.NET Core Web API  
- Entity Framework Core (Oracle)  
- Serilog para logging  
- Swagger / Swashbuckle para documentação  
- Autenticação: JWT + API Key  
- Banco de Dados: Oracle  
- Versionamento de API com `Microsoft.AspNetCore.Mvc.Versioning`

## 🛠️ Pré‑requisitos  
- SDK .NET 8.0 instalado  
- Oracle Database (connect string configurada em `appsettings.json`)  
- (Opcional) Visual Studio ou VS Code  

## Instalação / Execução Local  
1. Clone o repositório:  
   ```bash
   git clone <link‑do‑repositório>
   cd skillup‑api
