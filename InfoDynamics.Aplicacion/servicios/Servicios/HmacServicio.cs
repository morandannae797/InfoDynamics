using AutoMapper;
using InfoDynamics.Aplicacion.dtos;

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
namespace InfoDynamics.Aplicacion.servicios
{
    public class HmacServicio: IHmacServicio
    {
        private readonly byte[] _secret;
        public HmacServicio(IConfiguration config)
        {
            var secret = config["auth:HmacSecretKey"];
            if (string.IsNullOrEmpty(secret))
                throw new InvalidOperationException("Falta auth:hmacsecretp0 en configuración");
            _secret = Encoding.UTF8.GetBytes(secret);
        }
        public bool VerifySignature(loginDto request, string providedsignature)
        {
            if (string.IsNullOrEmpty(providedsignature))
                throw new InvalidOperationException("No se encontró la configuración 'auth:hmacsecretp0'");
            if (request == null || string.IsNullOrEmpty(providedsignature))
                return false;
            
            var payload = $"{request.identificador}:{request.contrasena}";
           

            using var hmac = new System.Security.Cryptography.HMACSHA256(_secret);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            var computedSignature = Convert.ToBase64String(hash);
            return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedSignature),
            Encoding.UTF8.GetBytes(providedsignature));
        }
    }
    }

