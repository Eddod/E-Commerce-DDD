﻿using Application.Abstractions.IServices;
using Domain.Entities.Customers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Persistance.Service;

internal sealed class JwtProviderService : IJwtProviderService
{
    private readonly JwtOptions _options;
    public JwtProviderService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string Generate(Customer customer)
    {
        var claims = new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, customer.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, customer.Email.Value),
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        string tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);
        return tokenValue;

    }
}
