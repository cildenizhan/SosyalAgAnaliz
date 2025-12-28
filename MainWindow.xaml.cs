using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SocialNetworkAnalysis.Services;
using SocialNetworkAnalysis.Models;

namespace SocialNetworkAnalysis
{
    public partial class MainWindow : Window
    {
        private GraphService _graphService;
        private FileService _fileService;

        public MainWindow()
        {
            InitializeComponent();
            LoadData(); // Program açılınca verileri yükle
        }

        private void LoadData()
        {
            _fileService = new FileService();
            
            // Dosya yolunu bul ve yükle
            string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "data.csv");
            if (!System.IO.File.Exists(path)) path = "data.csv";

            _graphService = _fileService.LoadGraph(path);

            if (_graphService.Nodes.Count > 0)
            {
                AssignRandomPositions(); // Düğümlere rastgele koordinat ver
                DrawGraph(); // Ekrana çiz
            }
            else
            {
                MessageBox.Show("Veri yüklenemedi!");
            }
        }

        // CSV'de koordinat olmadığı için rastgele dağıtıyoruz
        private void AssignRandomPositions()
        {
            Random rnd = new Random();
            double width = MainCanvas.ActualWidth > 0 ? MainCanvas.ActualWidth : 800;
            double height = MainCanvas.ActualHeight > 0 ? MainCanvas.ActualHeight : 600;

            foreach (var node in _graphService.Nodes.Values)
            {
                // Kenarlara çok yapışmasın diye +50 ve -100 yapıyoruz
                node.X = rnd.Next(50, (int)width - 50);
                node.Y = rnd.Next(50, (int)height - 50);
            }
        }

        private void DrawGraph()
        {
            MainCanvas.Children.Clear(); // Önce temizle

            // 1. ADIM: Önce Çizgileri (Kenarları) Çiz (Arkada kalsınlar)
            foreach (var edge in _graphService.Edges)
            {
                Line line = new Line
                {
                    X1 = edge.Source.X + 15, // +15 merkeze hizalamak için (Düğüm genişliği 30 olacak)
                    Y1 = edge.Source.Y + 15,
                    X2 = edge.Target.X + 15,
                    Y2 = edge.Target.Y + 15,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 2
                };
                MainCanvas.Children.Add(line);
            }

            // 2. ADIM: Düğümleri (Kutucukları) Çiz
            foreach (var node in _graphService.Nodes.Values)
            {
                // Düğümü temsil eden yuvarlak (Ellipse)
                Ellipse ellipse = new Ellipse
                {
                    Width = 30,
                    Height = 30,
                    Fill = Brushes.LightBlue,
                    Stroke = Brushes.Black,
                    Tag = node // Tıklayınca hangi node olduğunu anlamak için içine gizliyoruz
                };

                // Tıklama Olayı
                ellipse.MouseLeftButtonDown += Node_Clicked;

                // Konumlandır
                Canvas.SetLeft(ellipse, node.X);
                Canvas.SetTop(ellipse, node.Y);

                // Düğümün üzerine ismini yazalım
                TextBlock textBlock = new TextBlock
                {
                    Text = node.Name,
                    FontWeight = FontWeights.Bold
                };
                Canvas.SetLeft(textBlock, node.X);
                Canvas.SetTop(textBlock, node.Y - 15); // Yuvarlağın biraz üstüne

                MainCanvas.Children.Add(ellipse);
                MainCanvas.Children.Add(textBlock);
            }
        }

        // Düğüme tıklanınca çalışacak kod
        private void Node_Clicked(object sender, MouseButtonEventArgs e)
        {
            Ellipse ellipse = (Ellipse)sender;
            UserNode node = (UserNode)ellipse.Tag; // Gizlediğimiz veriyi geri alıyoruz

            // Sağ paneldeki yazıları güncelle
            TxtName.Text = node.Name;
            TxtActivity.Text = node.Activity.ToString();
            TxtInteraction.Text = node.Interaction.ToString();
            TxtConnection.Text = node.ConnectionCount.ToString();

            // Tıklananı Kırmızı yap, diğerlerini eski rengine döndür
            foreach (var child in MainCanvas.Children)
            {
                if (child is Ellipse el) el.Fill = Brushes.LightBlue;
            }
            ellipse.Fill = Brushes.Red;
            
            // Olayın Canvas'a taşmasını engelle
            e.Handled = true;
        }

        private void BtnRedraw_Click(object sender, RoutedEventArgs e)
        {
            AssignRandomPositions();
            DrawGraph();
        }
        
        // Boşluğa tıklayınca seçimi kaldır
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            foreach (var child in MainCanvas.Children)
            {
                if (child is Ellipse el) el.Fill = Brushes.LightBlue;
            }
            TxtName.Text = "-";
        }
    }
}