using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;

namespace WebApplication3
{
    public class SigningConfigurations
    {
        
        public SecurityKey Key { get; }
        public SigningCredentials SigningCredentials { get; }

        public SigningConfigurations()
        {
            using (var provider = new RSACryptoServiceProvider(2048))
            {
                //Armazena a Chave de Criptografia em Key
                Key = new RsaSecurityKey(provider.ExportParameters(true));
            }
            //SigningCredentials Recebe a Chave de Criptografia e o algoritmo de geração de token
            SigningCredentials = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
        }
    }
}
