using iTextSharp.text;
using iTextSharp.text.pdf;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using OxyPlot.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Menu;
using BarItem = OxyPlot.Series.BarItem;
using Font = iTextSharp.text.Font;
using Paragraph = iTextSharp.text.Paragraph;
using Path = System.IO.Path;

namespace MarcenariaMorais
{
    /// <summary>
    /// Interação lógica para Financeiro_L.xam
    /// </summary>
    public partial class Financeiro_L : Page
    {
        private DataTable daO; // Dados Originais
        private double faturamento;
        private double gastos;

        public Financeiro_L()
        {
            InitializeComponent();
            CarregarTabela();
            this.DataContext = this;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            tb_pesquisa.TextChanged += AplicarFiltro;
        }

        public void CarregarTabela()
        {
            daO = Financeiro.ListarTodos();
            dt_tabela.ItemsSource = daO?.DefaultView;

            faturamento = 0;
            gastos = 0;
            double total = 0;

            foreach (DataRowView row in dt_tabela.Items)
            {
                double val = Convert.ToDouble(row["Fin_mov_val"]);
                if (val > 0)
                    faturamento += val;
                if (val < 0)
                    gastos += Math.Abs(val);
            }

            total = faturamento - gastos;

            lb_ganhos.Text = faturamento.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"));
            lb_gastos.Text = gastos.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"));
            lb_total.Text = total.ToString("C2", CultureInfo.GetCultureInfo("pt-BR"));
        }

        private void btn_adicionar_Click(object sender, RoutedEventArgs e)
        {
            NavigationHandler.SetAndRefresh("FinanceiroA");
        }

        private void btn_alterar_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione um lançamento na lista para alterar!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var row = (DataRowView)dt_tabela.SelectedItem;
            int id = Convert.ToInt32(row["Fin_id"]);
            string desc = row["Fin_mov_desc"].ToString().Trim();
            double valor = Double.Parse(row["Fin_mov_val"].ToString());
            DateTime data = DateTime.Parse(row["Fin_data"].ToString());

            Debug.WriteLine($"Valor passado para a página de alterar: {valor}");

            Financeiro fin = new Financeiro
            {
                Id = id,
                Desc = desc,
                Valor = valor,
                Data = data
            };

            NavigationHandler.SetAndRefresh("FinanceiroE", new object[] { fin });
        }

        private void btn_excluir_Click(object sender, RoutedEventArgs e)
        {
            if (dt_tabela.SelectedItem == null)
            {
                MessageBox.Show("Selecione um lançamento para excluir!", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var row = (DataRowView)dt_tabela.SelectedItem;
            int id = Convert.ToInt32(row["Fin_id"]);

            Financeiro fin = new Financeiro
            {
                Id = id,
                Desc = row["Fin_mov_desc"].ToString()
            };

            bool? ok = fin.Excluir();
            if (ok == true)
            {
                CarregarTabela();
            }
        }

        private void AplicarFiltro(object sender, TextChangedEventArgs e)
        {
            if (daO == null) return;

            string textoPesquisa = tb_pesquisa.Text.Trim();

            if (string.IsNullOrWhiteSpace(textoPesquisa))
            {
                // Se não há texto, mostra tudo
                daO.DefaultView.RowFilter = string.Empty;
            }
            else
            {
                // Escapa aspas simples para evitar erros de sintaxe
                textoPesquisa = textoPesquisa.Replace("'", "''");

                // Filtro que busca em múltiplas colunas
                // Para colunas numéricas, usa CONVERT. Para texto, usa LIKE direto
                string filtro = $@"CONVERT(Fin_id, 'System.String') LIKE '%{textoPesquisa}%' OR Fin_mov_desc LIKE '%{textoPesquisa}%'";

                try
                {
                    daO.DefaultView.RowFilter = filtro;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao filtrar: {ex.Message}");
                    daO.DefaultView.RowFilter = string.Empty;
                }
            }
        }


        // CÓDIGO DE GERAÇÃO DE PDF

        private List<Financeiro> listaMovimentos = new List<Financeiro>();

        //Faz o gráfico de Pizza pegando como comparação o mês atual
        public string ConfigurarGraficoPizza()
        {
            //pega as datas que são referentes ao mês atual
            int mesAtual = DateTime.Now.Month;
            int anoAtual = DateTime.Now.Year;

            var movimentosDoMes = listaMovimentos.Where(m => m.Data.Month == mesAtual && m.Data.Year == anoAtual).ToList();

            //Soma todos os valores referente desse mês na lista "listaMovimentos"
            decimal ganhosTotal = (decimal)movimentosDoMes.Where(i => i.Valor > 0).Sum(i => i.Valor);
            decimal gastosTotal = (decimal)movimentosDoMes.Where(i => i.Valor < 0).Sum(i => Math.Abs(i.Valor));

            //Cria o modelo do Gráfico de pizza (Pie Chart no inglês)
            var model = new PlotModel { };

            //Características do Pie Chart
            var pieSeries = new PieSeries()
            {
                InsideLabelPosition = 0.5,
                StrokeThickness = 1.0,
                AngleSpan = 360,
                StartAngle = 0,
                TickHorizontalLength = 0,
                TickRadialLength = 0,
                FontSize = 16
            };

            //Cria os Pedaços do Pie Charts colocando os valores corretos com a configuração da moeda real (:C)
            pieSeries.Slices.Add(new PieSlice($"Entrada ({ganhosTotal:C})", (double)ganhosTotal) { IsExploded = true, Fill = OxyColors.Green });
            pieSeries.Slices.Add(new PieSlice($"Saída ({gastosTotal:C})", (double)gastosTotal) { IsExploded = true, Fill = OxyColors.Red });

            //Adiciona o gráfico de pizza
            model.Series.Add(pieSeries);

            //PlotView.Model = model;

            //Método para criar a imagem do Gráfico de Pizza

            //Adiciona as configurações da imagem gerada do Gráfico
            var pngExporter = new OxyPlot.Wpf.PngExporter
            {
                Width = 600,
                Height = 400,
                Resolution = 150
            };

            //Cria a pasta de destino com o nome "Gráficos"
            string pastaGrafica = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Gráficos");

            //verifica se a pasta  existe
            if (!Directory.Exists(pastaGrafica)) { Directory.CreateDirectory(pastaGrafica); }

            //Cria o caminho temporário da imagem já com o nome
            string caminhoTemp = Path.Combine(pastaGrafica, $"graficoPizza_{Guid.NewGuid()}.png");

            FileStream fileStream = null;

            //Cria o arquivo e exporta
            fileStream = File.Create(caminhoTemp);
            pngExporter.Export(model, fileStream);

            fileStream.Close();


            return caminhoTemp;
        }

        //Grafico comparando os gastos e ganhos no mês
        public PlotModel CriarGraficoBarrasPorMes(List<Financeiro> listaMovimentos)
        {
            //Cria o modelo do Gráfico de barras
            var model2 = new PlotModel { };

            // Agrupa os elementos da lista por mês/ano
            var agrupadoPorMes = listaMovimentos
                .GroupBy(m => new { m.Data.Year, m.Data.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .ToList();


            // Eixo das categorias (meses) >>> eixo vertical (esquerda)
            var eixoCategorias = new OxyPlot.Axes.CategoryAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Left,
                Key = "Meses"
            };

            // Adiciona os labels individualmente
            foreach (var grupo in agrupadoPorMes)
            {
                eixoCategorias.Labels.Add($"{grupo.Key.Month:00}/{grupo.Key.Year}");
            }
            model2.Axes.Add(eixoCategorias);

            // Eixo de valores >>> horizontal (baixo)
            var eixoValor = new LinearAxis
            {
                Position = OxyPlot.Axes.AxisPosition.Bottom,
                Title = "Valores (R$)",
                Minimum = 0.0,
                MinimumPadding = 0.1,
                MaximumPadding = 0.1
            };
            model2.Axes.Add(eixoValor);

            // Listas de BarItem
            var ganhosPorMes = new List<BarItem>();
            var gastosPorMes = new List<BarItem>();
            var diferencaPorMes = new List<BarItem>();

            //Soma todos os elementos, separa se são ganhos (números positivo) e gastos (números negativos) e os adiciona na lista
            //E calcula a diferença entre eles
            foreach (var grupo in agrupadoPorMes)
            {
                decimal ganhosTotal = (decimal)grupo.Where(m => m.Valor > 0).Sum(m => m.Valor);
                decimal gastosTotal = (decimal)grupo.Where(m => m.Valor < 0).Sum(m => Math.Abs(m.Valor));
                decimal diferenca = ganhosTotal - gastosTotal;

                ganhosPorMes.Add(new BarItem { Value = (double)ganhosTotal });
                gastosPorMes.Add(new BarItem { Value = (double)gastosTotal });
                diferencaPorMes.Add(new BarItem { Value = (double)diferenca });
            }

            // Cria as séries de barras (BarSeries), cada uma para uma categoria
            var serieGanhos = new BarSeries
            {
                Title = "Receita",
                ItemsSource = ganhosPorMes,
                FillColor = OxyColors.Green,
                LabelFormatString = "{0:C2}",
                LabelPlacement = LabelPlacement.Outside,
                FontSize = 16,
                LegendKey = "ganhoLegendas",
                BarWidth = 0.8
            };

            var serieGastos = new BarSeries
            {
                Title = "Despesas",
                ItemsSource = gastosPorMes,
                FillColor = OxyColors.Red,
                LabelFormatString = "{0:C2}",
                LabelPlacement = LabelPlacement.Outside,
                FontSize = 16,
                LegendKey = "gastoLegendas",
                BarWidth = 0.8
            };

            var serieDiferenca = new BarSeries
            {
                Title = "Lucro Líquido",
                ItemsSource = diferencaPorMes,
                FillColor = OxyColors.Blue,
                LabelFormatString = "{0:C2}",
                LabelPlacement = LabelPlacement.Outside,
                FontSize = 16,
                LegendKey = "diferencaLegendas",
                BarWidth = 0.8
            };

            //Cria as legendas do gráfico de barras
            var gastoLegendas = new Legend
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomRight,
                FontSize = 12,
                Key = "gastoLegendas"
            };

            var ganhoLegendas = new Legend
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomCenter,
                FontSize = 12,
                Key = "ganhoLegendas"
            };

            var diferencaLegendas = new Legend
            {
                LegendPlacement = LegendPlacement.Outside,
                LegendPosition = LegendPosition.BottomLeft,
                FontSize = 12,
                Key = "diferencaLegendas"
            };

            // Adiciona as séries ao modelo
            model2.Series.Add(serieGanhos);
            model2.Series.Add(serieGastos);
            model2.Series.Add(serieDiferenca);
            model2.Legends.Add(ganhoLegendas);
            model2.Legends.Add(gastoLegendas);
            model2.Legends.Add(diferencaLegendas);

            return model2;
        }

        //Cria a imagem do gráfico de barra
        public string ExportarGraficoBarraParaPNG(PlotModel model2, string nomeArquivo)
        {
            string pastaGrafica = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Gráficos");
            if (!Directory.Exists(pastaGrafica)) Directory.CreateDirectory(pastaGrafica);

            string caminhoTemp2 = Path.Combine(pastaGrafica, $"{nomeArquivo}_{Guid.NewGuid()}.png");

            using (var fileStream = File.Create(caminhoTemp2))
            {
                var pngExporter = new OxyPlot.Wpf.PngExporter
                {
                    Width = 900,
                    Height = 600
                };
                pngExporter.Export(model2, fileStream);
            }

            return caminhoTemp2;
        }

        //Método que cria o PDF
        public void CriarPDF(string caminhoTemp, string caminhoTemp2)
        {
            string dataPDF = DateTime.Now.ToString("MMyyyy"); //´pega da atual

            //Cria e adiciona as margens do PDF
            Document doc = new Document(iTextSharp.text.PageSize.A4);
            doc.SetMargins(40, 40, 40, 80);
            doc.AddCreationDate();

            //Configuração da fonte principal do pdf
            BaseFont FontePrincipal = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);

            //configuração das fontes dos títulos e a fonte comum
            Font Titulo = new Font(FontePrincipal, 16, Font.BOLD);
            Font fonte = new Font(FontePrincipal, 12, Font.BOLD);


            //Pega o caminho correto do diretório Documentos do computador e coloca o nome correto do PDF
            string caminhoPDF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "relatorio_financeiro_mensal_" + dataPDF + ".pdf");
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(caminhoPDF, FileMode.Create)); //cria o arquivo


            // Parágrafo de título "FINANCEIRO"
            Paragraph TituloFinanceiro = new Paragraph("TABELA DE CONSULTA FINANCEIRA\n\n", Titulo);
            TituloFinanceiro.Alignment = iTextSharp.text.Element.ALIGN_CENTER;

            // Cria a tabela que irá receber as movimentações
            PdfPTable tabela = new PdfPTable(4);
            tabela.AddCell(new PdfPCell(new Paragraph("Descrição", fonte)));
            tabela.AddCell(new PdfPCell(new Paragraph("Tipo", fonte)));
            tabela.AddCell(new PdfPCell(new Paragraph("Valor", fonte)));
            tabela.AddCell(new PdfPCell(new Paragraph("Data", fonte)));

            //Faz uma procura na lista e adiciona as informações na lista comparando se são gastos ou recebmento
            foreach (var mov in listaMovimentos)
            {
                string tipoMovimento = mov.Valor < 0 ? "Gasto" : "Recebimento";
                tabela.AddCell(mov.Desc);
                tabela.AddCell(tipoMovimento);
                tabela.AddCell(mov.Valor.ToString("C2"));
                tabela.AddCell(mov.Data.ToString("dd/MM/yyyy"));
            }


            // Parágrafo de título "GRÁFICO COMPARATIVO DE MÊS"
            Paragraph TituloPizza = new Paragraph("GRÁFICO COMPARATIVO DE MÊS\n", Titulo);
            TituloPizza.Alignment = iTextSharp.text.Element.ALIGN_CENTER;

            //Cria a imagem que irá comportar a imagem do gráfico de pizza
            iTextSharp.text.Image imgPizza = iTextSharp.text.Image.GetInstance(caminhoTemp);
            imgPizza.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            imgPizza.ScaleToFit(300f, 200f);


            // Parágrafo de título "RECEITA, DESPESAS E DIFERENÇA POR MÊS"
            Paragraph TituloBarra = new Paragraph("RECEITA, DESPESAS E DIFERENÇA POR MÊS\n", Titulo);
            TituloBarra.Alignment = iTextSharp.text.Element.ALIGN_CENTER;

            //Cria a imagem que irá comportar a imagem do gráfico de barras
            iTextSharp.text.Image imgBarra = iTextSharp.text.Image.GetInstance(caminhoTemp2);
            imgBarra.Alignment = iTextSharp.text.Element.ALIGN_CENTER;
            imgBarra.ScaleToFit(500f, 350f);


            //Abre o documento para edição
            doc.Open();

            //Adiciona os itens no PDF
            //Primeira Página
            doc.Add(TituloFinanceiro);
            doc.Add(tabela);
            //Segunda página
            doc.NewPage();
            doc.Add(TituloPizza);
            doc.Add(imgPizza);
            //terceira página
            doc.NewPage();
            doc.Add(TituloBarra);
            doc.Add(imgBarra);

            //Fecha o documento
            doc.Close();

            //Abre automaticamente o pdf (Recurso de teste)
            System.Diagnostics.Process.Start(caminhoPDF);

            // Apaga os arquivos temporário da imagem
            try
            {
                File.Delete(caminhoTemp);
                File.Delete(caminhoTemp2);
            }
            catch { }
        }

        //Chama a os métodos para a criação do pdf
        private void GerarPDF(object sender, RoutedEventArgs e)
        {
            listaMovimentos = dt_tabela.Items.Cast<DataRowView>().Select(m => new Financeiro()
            {
                Id = Convert.ToInt32(m["Fin_id"]),
                Desc = m["Fin_mov_desc"].ToString(),
                Valor = Convert.ToDouble(m["Fin_mov_val"]),
                Data = Convert.ToDateTime(Convert.ToDateTime(m["Fin_data"]))

            }).ToList();

            string caminhoImgPizza = ConfigurarGraficoPizza();

            var modeloGraficoBarra = CriarGraficoBarrasPorMes(listaMovimentos);
            string caminhoImgBarra = ExportarGraficoBarraParaPNG(modeloGraficoBarra, "graficoBarra");

            //PlotView.Model = modeloGraficoBarra;

            CriarPDF(caminhoImgPizza, caminhoImgBarra);
        }
    }
}
