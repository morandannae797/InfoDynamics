using System;
using System.Collections.Generic;
using System.Text;

using System.Security.Cryptography;


namespace InfoDynamics.Aplicacion.servicios.Servicios
{
    public class GeneradorContrasena
    {
        private const string Chars =
        "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@$?_-";

        public static string Generarcontrasenatemporal(int length = 12)
        {
            var contra = new char[length];

            for (int i = 0; i < length; i++)
            {
                contra[i] = Chars[RandomNumberGenerator.GetInt32(Chars.Length)];
            }

            return new string(contra);
        }

        public class GeneradorReseteo
        {
            private const string Chars =
            "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz23456789!@$?_-";

            public static string Generarcontrasenatemporal(int length = 6)
            {
                var RESET = new char[length];

                for (int i = 0; i < length; i++)
                {
                    RESET[i] = Chars[RandomNumberGenerator.GetInt32(Chars.Length)];
                }

                return new string(RESET);
            }
        }
    }
}
