using System;
using System.Collections.Generic;
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
        private AlgorithmService _algoService;
        
        private UserNode _firstSelected = null;
        private UserNode _secondSelected = null;

        public MainWindow()
        {
            InitializeComponent();
            _algoService = new AlgorithmService();
            LoadData();
        }

        private void LoadData()
        {
            _fileService = new FileService();
            string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "data.csv");
            if (!System.IO.File.Exists(path)) path = "data.csv";

            _graphService = _fileService.LoadGraph(path);

            if (_graphService.Nodes.Count > 0)
            {
                AssignRandomPositions();
                DrawGraph();
            }
        }

        private void AssignRandomPositions()
        {
            Random rnd = new Random();
            double width = MainCanvas.ActualWidth > 0 ? MainCanvas.ActualWidth : 800;
            double height = MainCanvas.ActualHeight > 0 ? MainCanvas.ActualHeight : 600;

            foreach (var node in _graphService.Nodes.Values)
            {
                node.X = rnd.Next(50, (int)width - 50);
                node.Y = rnd.Next(50, (int)height - 50);
            }
        }

        private void DrawGraph()
        {
            MainCanvas.Children.Clear();

            
            foreach (var edge in _graphService.Edges)
            {
                Line line = new Line
                {
                    X1 = edge.Source.X + 15,
                    Y1 = edge.Source.Y + 15,
                    X2 = edge.Target.X + 15,
                    Y2 = edge.Target.Y + 15,
                    Stroke = Brushes.Gray,
                    StrokeThickness = 2,
                    Tag = edge 
                };
                MainCanvas.Children.Add(line);
            }

            
            foreach (var node in _graphService.Nodes.Values)
            {
                Ellipse ellipse = new Ellipse
                {
                    Width = 30, Height = 30,
                    Fill = Brushes.LightBlue, Stroke = Brushes.Black,
                    Tag = node
                };
                ellipse.MouseLeftButtonDown += Node_Clicked;

                Canvas.SetLeft(ellipse, node.X);
                Canvas.SetTop(ellipse, node.Y);

                TextBlock textBlock = new TextBlock { Text = node.Name, FontWeight = FontWeights.Bold };
                Canvas.SetLeft(textBlock, node.X);
                Canvas.SetTop(textBlock, node.Y - 15);

                MainCanvas.Children.Add(ellipse);
                MainCanvas.Children.Add(textBlock);
            }
        }

        private void Node_Clicked(object sender, MouseButtonEventArgs e)
        {
            Ellipse ellipse = (Ellipse)sender;
            UserNode node = (UserNode)ellipse.Tag;

            TxtName.Text = node.Name;
            TxtActivity.Text = node.Activity.ToString();
            TxtInteraction.Text = node.Interaction.ToString();
            TxtConnection.Text = node.ConnectionCount.ToString();
            
            if (_firstSelected == null)
            {
                _firstSelected = node;
                TxtSelectedUsers.Text = $"1. Seçilen: {node.Name}\n2. Seçin...";
                ellipse.Fill = Brushes.Yellow; 
            }
            else if (_secondSelected == null && node != _firstSelected)
            {
                _secondSelected = node;
                TxtSelectedUsers.Text = $"1. {_firstSelected.Name}\n2. {_secondSelected.Name}";
                ellipse.Fill = Brushes.Yellow; 
            }
            else
            {
                ResetSelection();
                _firstSelected = node;
                TxtSelectedUsers.Text = $"1. Seçilen: {node.Name}\n2. Seçin...";
                ellipse.Fill = Brushes.Yellow;
            }
            e.Handled = true;
        }

        private void ResetSelection()
        {
            _firstSelected = null;
            _secondSelected = null;
            TxtSelectedUsers.Text = "Seçilen: Yok";
            DrawGraph(); 
        }

        private void BtnPopular_Click(object sender, RoutedEventArgs e)
        {
            var popular = _algoService.FindMostPopularUser(_graphService);
            if (popular != null)
            {
                MessageBox.Show($"En Popüler Kişi: {popular.Name}\nBağlantı Sayısı: {popular.ConnectionCount}");
                foreach (var child in MainCanvas.Children)
                {
                    if (child is Ellipse el && el.Tag == popular)
                    {
                        el.Fill = Brushes.Gold;
                        el.Width = 40; el.Height = 40; 
                    }
                }
            }
        }

        private void BtnFindPath_Click(object sender, RoutedEventArgs e)
        {
            if (_firstSelected == null || _secondSelected == null)
            {
                MessageBox.Show("Lütfen haritadan iki kişi seçin!");
                return;
            }

            var path = _algoService.FindShortestPath(_graphService, _firstSelected, _secondSelected);

            if (path == null)
            {
                MessageBox.Show("Bu iki kişi arasında bağlantı yok!");
                return;
            }

            
            DrawGraph();
            HighlightSelection();

            
            foreach (var node in path)
            {
                foreach (var child in MainCanvas.Children)
                    if (child is Ellipse el && el.Tag == node) el.Fill = Brushes.LimeGreen;
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                var u1 = path[i];
                var u2 = path[i + 1];
                foreach (var child in MainCanvas.Children)
                {
                    if (child is Line line && line.Tag is Edge edge)
                    {
                        if ((edge.Source == u1 && edge.Target == u2) || (edge.Source == u2 && edge.Target == u1))
                        {
                            line.Stroke = Brushes.LimeGreen;
                            line.StrokeThickness = 5;
                        }
                    }
                }
            }
        }

        
        private void BtnDijkstra_Click(object sender, RoutedEventArgs e)
        {
            if (_firstSelected == null || _secondSelected == null)
            {
                MessageBox.Show("Lütfen haritadan iki kişi seçin!");
                return;
            }

            var path = _algoService.FindPathDijkstra(_graphService, _firstSelected, _secondSelected);

            if (path == null)
            {
                MessageBox.Show("Yol bulunamadı!");
                return;
            }

            
            DrawGraph();
            HighlightSelection();

            
            foreach (var node in path)
            {
                foreach (var child in MainCanvas.Children)
                    if (child is Ellipse el && el.Tag == node) el.Fill = Brushes.Orange;
            }

            for (int i = 0; i < path.Count - 1; i++)
            {
                var u1 = path[i];
                var u2 = path[i + 1];
                foreach (var child in MainCanvas.Children)
                {
                    if (child is Line line && line.Tag is Edge edge)
                    {
                        if ((edge.Source == u1 && edge.Target == u2) || (edge.Source == u2 && edge.Target == u1))
                        {
                            line.Stroke = Brushes.Orange;
                            line.StrokeThickness = 5;
                        }
                    }
                }
            }
            MessageBox.Show($"Dijkstra (Mesafe) Yolu Bulundu!\nAdım Sayısı: {path.Count - 1}");
        }

        
        private void HighlightSelection()
        {
            foreach (var child in MainCanvas.Children)
            {
                if (child is Ellipse el && (el.Tag == _firstSelected || el.Tag == _secondSelected))
                {
                    el.Fill = Brushes.Yellow;
                }
            }
        }

        private void BtnRedraw_Click(object sender, RoutedEventArgs e)
        {
            AssignRandomPositions();
            DrawGraph();
            ResetSelection();
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            DrawGraph();
            ResetSelection();
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }
    }
}