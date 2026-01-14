using System;
using System.Security.Cryptography;
using System.Text;

namespace MarcenariaMorais
{
    public static class Criptografia
    {
        // ⚠️ MESMA CHAVE DO IMPORTADOR
        private const string ChaveSecreta = "SUA_CHAVE_AQUI";

        private static byte[] ObterChave()
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(ChaveSecreta));
            }
        }

        public static string Criptografar(string textoOriginal)
        {
            if (string.IsNullOrEmpty(textoOriginal)) return textoOriginal;

            using (var aes = Aes.Create())
            {
                aes.Key = ObterChave();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.GenerateIV();

                var textoBytes = Encoding.UTF8.GetBytes(textoOriginal);
                using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    var cipherBytes = encryptor.TransformFinalBlock(textoBytes, 0, textoBytes.Length);

                    var resultado = new byte[aes.IV.Length + cipherBytes.Length];
                    Buffer.BlockCopy(aes.IV, 0, resultado, 0, aes.IV.Length);
                    Buffer.BlockCopy(cipherBytes, 0, resultado, aes.IV.Length, cipherBytes.Length);

                    return Convert.ToBase64String(resultado);
                }
            }
        }

        public static string Descriptografar(string textoCriptografado)
        {
            if (string.IsNullOrEmpty(textoCriptografado)) return textoCriptografado;

            try
            {
                var dadosCompletos = Convert.FromBase64String(textoCriptografado);
                var iv = new byte[16];
                var cipher = new byte[dadosCompletos.Length - iv.Length];

                Buffer.BlockCopy(dadosCompletos, 0, iv, 0, iv.Length);
                Buffer.BlockCopy(dadosCompletos, iv.Length, cipher, 0, cipher.Length);

                using (var aes = Aes.Create())
                {
                    aes.Key = ObterChave();
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        var textoBytes = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
                        return Encoding.UTF8.GetString(textoBytes);
                    }
                }
            }
            catch
            {
                return textoCriptografado; // Se falhar, retorna como está
            }
        }

        public static string GerarHash(string texto)
        {
            using (var sha = SHA256.Create())
            {
                var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(texto));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}
