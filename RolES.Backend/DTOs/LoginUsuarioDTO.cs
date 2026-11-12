namespace RolES.Backend.DTOs;

public class LoginUsuarioDTO
{
    //classe é composta po métodos e proptiedades
    //para criar um dto precisa criar apenas propriedades


    public string Senha { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}