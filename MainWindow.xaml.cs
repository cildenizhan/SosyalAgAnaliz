using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using SocialNetworkAnalysis.Services;
using SocialNetworkAnalysis.Models;
using SocialNetworkAnalysis.Algorithms;

namespace SocialNetworkAnalysis
{
    public partial class MainWindow : Window
    {
        private IGraphService? _graphService;
        private FileService? _fileService;
        private AlgorithmService _algoService;
        
        private Node? _firstSelected = null;
        private Node? _secondSelected = null;

        public MainWindow()
        {
            InitializeComponent();
            _algoService = new AlgorithmService();
            LoadData();
        }

        private void LoadData()
        {
            _fileService = new FileService();
            string fileName = "data.csv";
            
            
            string basePath = System.AppDomain.CurrentDomain.BaseDirectory; 
            string projectPath = System.IO.Directory.GetParent(basePath)?.Parent?.Parent?.FullName ?? basePath; 
            
            string finalPath = "";

            if (System.IO.File.Exists(System.IO.Path.Combine(basePath, fileName)))
            {
                finalPath = System.IO.Path.Combine(basePath, fileName);
            }
            else if (System.IO.File.Exists(System.IO.Path.Combine(projectPath, fileName)))
            {
                finalPath = System.IO.Path.Combine(projectPath, fileName);
            }

            
            if (string.IsNullOrEmpty(finalPath)) return;

            
            _graphService = _fileService.LoadGraph(finalPath);

            
            if (_graphService != null && _graphService.Nodes.Count > 0)
            {
                AssignRandomPositions();
                DrawGraph();
                LstTop5.ItemsSource = _algoService.GetTop5Users(_graphService);
            }
        }

        private void AssignRandomPositions()
        {
            if (_graphService == null) return;

            Random rnd = new Random();
            double width = MainCanvas.ActualWidth;
            double height = MainCanvas.ActualHeight;

            
            if (width <= 0 || double.IsNaN(width)) width = 800;
            if (height <= 0 || double.IsNaN(height)) height = 600;

            int padding = 50; 

            foreach (var node in _graphService.Nodes.Values)
            {
                int maxX = (int)width - padding;
                int maxY = (int)height - padding;

                if (maxX <= padding) maxX = padding + 50;
                if (maxY <= padding) maxY = padding + 50;

                node.X = rnd.Next(padding, maxX);
                node.Y = rnd.Next(padding, maxY);
            }
        }

        private void DrawGraph()
        {
            if (_graphService == null) return;
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

                TextBlock textBlock = new TextBlock 
                { 
                    Text = node.Name, 
                    FontWeight = FontWeights.Bold,
                    IsHitTestVisible = false 
                };
                
                Canvas.SetLeft(textBlock, node.X);
                Canvas.SetTop(textBlock, node.Y - 15);

                MainCanvas.Children.Add(ellipse);
                MainCanvas.Children.Add(textBlock);
            }
        }

        private void Node_Clicked(object sender, MouseButtonEventArgs e)
        {
            Ellipse ellipse = (Ellipse)sender;
            Node node = (Node)ellipse.Tag;

            TxtName.Text = node.Name;
            TxtActivity.Text = node.Activity.ToString();
            TxtInteraction.Text = node.Interaction.ToString();
            TxtConnection.Text = node.ConnectionCount.ToString();
            
            if (_firstSelected == null)
            {
                _firstSelected = node;
                TxtSelectedUsers.Text = $"1. {node.Name}\n2. Seçin...";
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
                TxtSelectedUsers.Text = $"1. {node.Name}\n2. Seçin...";
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

        
        private void BtnRedraw_Click(object sender, RoutedEventArgs e)
        {
            AssignRandomPositions();
            ResetSelection();
        }

        private void BtnPopular_Click(object sender, RoutedEventArgs e)
        {
            if (_graphService == null) return;
            
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
            if (_graphService == null || _firstSelected == null || _secondSelected == null)
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
            if (_graphService == null || _firstSelected == null || _secondSelected == null)
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

        private void BtnColoring_Click(object sender, RoutedEventArgs e)
        {
            if (_graphService == null) return;
            WelshPowell wp = new WelshPowell();
            var colors = wp.ColorGraph(_graphService);

            foreach (var child in MainCanvas.Children)
            {
                if (child is Line line) line.Stroke = Brushes.Gray;
            }

            var usedColors = new HashSet<string>();
            foreach (var kvp in colors)
            {
                var colorName = kvp.Value;
                usedColors.Add(colorName);
                var brush = (SolidColorBrush)new BrushConverter().ConvertFromString(colorName);
                foreach (var child in MainCanvas.Children)
                {
                    if (child is Ellipse el && el.Tag == kvp.Key) el.Fill = brush;
                }
            }
            MessageBox.Show($"Renklendirme Tamamlandı!\nKullanılan Renk Sayısı: {usedColors.Count}");
        }

        private void BtnDFS_Click(object sender, RoutedEventArgs e)
        {
            if (_graphService == null || _firstSelected == null)
            {
                MessageBox.Show("Lütfen haritadan başlangıç için bir kişi seçin!");
                return;
            }

            var visitedNodes = _algoService.DFS(_graphService, _firstSelected);
            DrawGraph();

            foreach (var node in visitedNodes)
            {
                foreach (var child in MainCanvas.Children)
                {
                    if (child is Ellipse el && el.Tag == node) el.Fill = Brushes.Purple; 
                }
            }
            MessageBox.Show($"DFS Tamamlandı!\nBu kişiden ulaşılabilen toplam kişi sayısı: {visitedNodes.Count}");
        }

        private void BtnComponents_Click(object sender, RoutedEventArgs e)
        {
            if (_graphService == null) return;
            var components = _algoService.FindConnectedComponents(_graphService);

            foreach (var child in MainCanvas.Children)
            {
                if (child is Line line) line.Stroke = Brushes.Gray;
            }

            string[] groupColors = { "Red", "Blue", "Green", "Orange", "Purple", "Cyan", "Magenta", "Brown" };
            int colorIndex = 0;

            foreach (var group in components)
            {
                string colorName = groupColors[colorIndex % groupColors.Length];
                var brush = (SolidColorBrush)new BrushConverter().ConvertFromString(colorName);

                foreach (var node in group)
                {
                    foreach (var child in MainCanvas.Children)
                    {
                        if (child is Ellipse el && el.Tag == node) el.Fill = brush;
                    }
                }
                colorIndex++;
            }
            MessageBox.Show($"Analiz Tamamlandı!\nToplam {components.Count} farklı grup (ayrık bileşen) bulundu.");
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_graphService == null) return;

            try
            {
                string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "saved_graph.json");
                _fileService.SaveGraphJson(_graphService, path);
                MessageBox.Show("Kayıt Başarılı!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt Hatası: {ex.Message}");
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            string path = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "saved_graph.json");
            
            if (!System.IO.File.Exists(path))
            {
                MessageBox.Show("Kaydedilmiş dosya bulunamadı.");
                return;
            }

            _graphService = _fileService.LoadGraphJson(path);
            DrawGraph();
            LstTop5.ItemsSource = _algoService.GetTop5Users(_graphService);
            MessageBox.Show("Yükleme Başarılı!");
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            DrawGraph();
            ResetSelection();
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
        }

        private void BtnAddNode_Click(object sender, RoutedEventArgs e)
        {
            if (_graphService == null) return;

            int newId = 1;
            if (_graphService.Nodes.Count > 0)
            {
                int maxId = 0;
                foreach(var key in _graphService.Nodes.Keys)
                {
                    if (key > maxId) maxId = key;
                }
                newId = maxId + 1;
            }

            Node newNode = new Node
            {
                Id = newId,
                Name = $"User_{newId}", 
                Activity = 0.5,
                Interaction = 10,
                ConnectionCount = 0
            };

            _graphService.AddNode(newNode);

            Random rnd = new Random();
            newNode.X = rnd.Next(50, (int)MainCanvas.ActualWidth - 50);
            newNode.Y = rnd.Next(50, (int)MainCanvas.ActualHeight - 50);

            DrawGraph();
            LstTop5.ItemsSource = _algoService.GetTop5Users(_graphService);
            MessageBox.Show($"Yeni kişi eklendi: {newNode.Name}");
        }

        private void BtnDeleteNode_Click(object sender, RoutedEventArgs e)
        {
            if (_graphService == null || _firstSelected == null)
            {
                MessageBox.Show("Lütfen silmek için haritadan bir kişi seçin (Sarı renkli).");
                return;
            }

            var result = MessageBox.Show($"{_firstSelected.Name} kişisini silmek istiyor musunuz?", "Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            
            if (result == MessageBoxResult.Yes)
            {
                _graphService.RemoveNode(_firstSelected.Id);
                ResetSelection(); 
                LstTop5.ItemsSource = _algoService.GetTop5Users(_graphService);
                MessageBox.Show("Kişi silindi.");
            }
        }

        private void BtnAddEdge_Click(object sender, RoutedEventArgs e)
        {
            if (_graphService == null || _firstSelected == null || _secondSelected == null)
            {
                MessageBox.Show("Lütfen bağlamak için iki kişi seçin.");
                return;
            }

            _graphService.AddEdge(_firstSelected.Id, _secondSelected.Id);
            DrawGraph();
            LstTop5.ItemsSource = _algoService.GetTop5Users(_graphService); 
            MessageBox.Show("Bağlantı kuruldu!");
        }

        private void BtnRemoveEdge_Click(object sender, RoutedEventArgs e)
        {
            if (_graphService == null || _firstSelected == null || _secondSelected == null)
            {
                MessageBox.Show("Lütfen bağlantısını koparmak için iki kişi seçin.");
                return;
            }

            _graphService.RemoveEdge(_firstSelected.Id, _secondSelected.Id);
            DrawGraph();
            LstTop5.ItemsSource = _algoService.GetTop5Users(_graphService);
            MessageBox.Show("Bağlantı koparıldı.");
        }
    }
}