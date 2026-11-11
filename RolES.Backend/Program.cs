using MySql.Data.MySqlClient;
using RolES.Backend.Dados;
using RolES.Backend.DTOs;
using RolES.Backend.Modelos;
using RolES.Backend.Servicos;
// rota de login
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Cadastro de Serviços Customizados
builder.Services.AddSingleton<BdConexao>();
builder.Services.AddScoped<UsuarioServicos>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseHttpsRedirection();
}

app.MapGet("/", () => // Metodo sem nome
{
    var dataResposta = DateTime.UtcNow;
    return Results.Ok(new
    {
        Mensagem = "Servidor está online.",
        Data = dataResposta
    });
});

app.MapPost("/usuario", async (CadastroUsuarioDto requestBody, UsuarioServicos uServicos) => //nosso servidor é o app
{
    try
    {
        var linhasAfetadas = await uServicos.AdicionarAsync(requestBody);
        
        if (linhasAfetadas > 0) return Results.Created();

        return Results.Conflict(new
            { Mensagem = "Houve um erro ao tentar criar o seu usuário. Tente novamente mais tarde." });
    }
    catch (MySqlException exception)
    {

        if (exception.Number == 1062)
            return Results.BadRequest(new
            {
                Mensagem = "0 e-maildp usuario já consta cadastrado."
            });
        throw;
    }
    catch
    {
        return Results.InternalServerError();
    }
});

app.MapPost("/login", async (LoginUsuarioDTO corpoRequisicao, UsuarioServicos uServicos) =>  
{
    try
    {
        var usuarioEncontrado = await uServicos.BuscarPorEmailAsync(corpoRequisicao.Email);
        if (usuarioEncontrado == null)
            return Results.NotFound(new
            {
                Mensagem = "usuario não encontrado"
            });

        if (corpoRequisicao.Senha != usuarioEncontrado.Senha)
            return Results.BadRequest(new
            {
                Mensagem = "email/senha estão incorretos."
                
            });
        return Results.Ok();

    }
    catch
    {
        return Results.InternalServerError();
    }
});

app.Run();