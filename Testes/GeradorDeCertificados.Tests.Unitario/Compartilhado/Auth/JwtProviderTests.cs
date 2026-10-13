using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GeradorCertificados.Dominio.Compartilhado.Auth;
using GeradorCertificados.WebApi.Compartilhado.Auth;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GeradorDeCertificados.Tests.Unitario.Compartilhado.Auth;

[TestClass]
public sealed class JwtProviderTests
{
    [TestMethod]
    public void Deve_GerarTokenValido_ComClaimsDoUsuario()
    {
        // Arrange
        const string issuer = "gerador-certificados-tests";
        const string audience = "gerador-certificados-client";
        const string key = "chave-de-teste-com-mais-de-32-caracteres";
        const int accessTokenMinutes = 15;
        Guid usuarioId = Guid.CreateVersion7();
        const string email = "teste@example.com";
        JwtProvider provider = new(Options.Create(new JwtOptions
        {
            Issuer = issuer,
            Audience = audience,
            Key = key,
            AccessTokenMinutes = accessTokenMinutes
        }));

        // Act
        AccessToken accessToken = provider.CriarToken(usuarioId, email);

        TokenValidationParameters validationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(
            accessToken.Token,
            validationParameters,
            out SecurityToken validatedToken);

        // Assert
        Assert.IsInstanceOfType<JwtSecurityToken>(validatedToken);
        Assert.AreEqual(usuarioId.ToString(), principal.FindFirstValue(ClaimTypes.NameIdentifier));
        Assert.AreEqual(email, principal.FindFirstValue(ClaimTypes.Email));
        Assert.AreEqual(issuer, validatedToken.Issuer);
        Assert.IsTrue(validatedToken.ValidTo > DateTime.UtcNow);
        Assert.IsTrue(validatedToken.ValidTo <= DateTime.UtcNow.AddMinutes(accessTokenMinutes));
        Assert.IsTrue(
            (accessToken.DataExpiracaoEmUtc - validatedToken.ValidTo).Duration()
                < TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Deve_RejeitarToken_QuandoChaveDeAssinaturaEstiverIncorreta()
    {
        // Arrange
        const string key = "chave-de-teste-com-mais-de-32-caracteres";
        Guid usuarioId = Guid.CreateVersion7();
        JwtProvider provider = new(Options.Create(new JwtOptions
        {
            Issuer = "gerador-certificados-tests",
            Audience = "gerador-certificados-client",
            Key = key
        }));
        AccessToken accessToken = provider.CriarToken(usuarioId, "teste@example.com");
        TokenValidationParameters validationParameters = new()
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("chave-de-teste-errada-com-mais-de-32-caracteres")),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Act & Assert
        Assert.Throws<SecurityTokenSignatureKeyNotFoundException>(() =>
            new JwtSecurityTokenHandler().ValidateToken(
                accessToken.Token,
                validationParameters,
                out _));
    }
}
