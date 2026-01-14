using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows;

namespace MarcenariaMorais
{
    internal class Banco
    {
        private string connStr = "server=162.241.2.71;port=3306;user=quaiat07_marcenaria_morais;password=21314151**mm;database=quaiat07_marcenaria_morais;SslMode=None;";
        
        public MySqlConnection Conectar()
        {
            var conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();
                return conn;
            }
            catch (MySqlException ex)
            {
                // Mostra um popup com o número e a descrição do erro MySQL
                MessageBox.Show(
                    "Erro ao conectar ao banco de dados",
                    "Erro de Conexão",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return null;
            }
            catch (Exception ex)
            {
                // Captura qualquer outro tipo de erro inesperado
                MessageBox.Show(
                    $"Erro inesperado ao tentar conectar:\n\n{ex.Message}",
                    "Erro de Sistema",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
                return null;
            }
        }
    }
}