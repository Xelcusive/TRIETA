using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IOPath = System.IO.Path;

namespace Trita
{
    public partial class ScanPdfWindow : Window
    {
        // Class để lưu thông tin mỗi trang
        private class PageInfo
        {
            public int PageNumber { get; set; }
            public string FilePath { get; set; }
            public Button PageButton { get; set; }
        }

        private List<PageInfo> _pages = new List<PageInfo>();
        private PageInfo _currentPage;

        public ScanPdfWindow()
        {
            InitializeComponent();
            // Tạo trang đầu tiên
            AddNewPage();
        }

        // Thêm trang mới
        private void AddPageButton_Click(object sender, RoutedEventArgs e)
        {
            AddNewPage();
        }

        private void AddNewPage()
        {
            int pageNumber = _pages.Count + 1;

            // Tạo button cho trang mới
            Button pageButton = new Button
            {
                Content = pageNumber.ToString(),
                Height = 60,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Background = Brushes.White,
                BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                BorderThickness = new Thickness(0, 0, 0, 1),
                Cursor = Cursors.Hand
            };

            // Tạo PageInfo
            PageInfo pageInfo = new PageInfo
            {
                PageNumber = pageNumber,
                FilePath = null,
                PageButton = pageButton
            };

            // Thêm context menu để xóa trang
            ContextMenu contextMenu = new ContextMenu();
            MenuItem deleteItem = new MenuItem { Header = "Xóa trang" };
            deleteItem.Click += (s, e) => DeletePage(pageInfo);
            contextMenu.Items.Add(deleteItem);
            pageButton.ContextMenu = contextMenu;

            // Click để chuyển trang
            pageButton.Click += (s, e) => SwitchToPage(pageInfo);

            _pages.Add(pageInfo);
            PageListPanel.Children.Add(pageButton);

            // Chuyển sang trang mới
            SwitchToPage(pageInfo);
        }

        // Chuyển sang trang khác
        private void SwitchToPage(PageInfo page)
        {
            // Reset màu tất cả các button
            foreach (var p in _pages)
            {
                p.PageButton.Background = Brushes.White;
            }

            // Highlight trang hiện tại
            page.PageButton.Background = new SolidColorBrush(Color.FromRgb(230, 230, 230));
            _currentPage = page;

            // Hiển thị nội dung trang
            DisplayCurrentPage();
        }

        // Hiển thị nội dung trang hiện tại
        private void DisplayCurrentPage()
        {
            MainContentArea.Children.Clear();

            if (string.IsNullOrEmpty(_currentPage.FilePath))
            {
                // Chưa có file → hiển thị vùng upload
                Grid uploadArea = CreateUploadArea();
                MainContentArea.Children.Add(uploadArea);
            }
            else
            {
                // Đã có file → hiển thị preview
                Grid previewArea = CreatePreviewArea();
                MainContentArea.Children.Add(previewArea);
            }
        }

        // Tạo vùng upload
        private Grid CreateUploadArea()
        {
            Grid grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 320,
                Height = 320,
                Cursor = Cursors.Hand,
                Background = Brushes.Transparent
            };

            grid.MouseLeftButtonDown += (s, e) => ChonFile();

            // Viền nét đứt
            Rectangle border = new Rectangle
            {
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection { 8, 4 },
                RadiusX = 10,
                RadiusY = 10
            };
            grid.Children.Add(border);

            // Nội dung
            StackPanel content = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TextBlock plusSign = new TextBlock
            {
                Text = "+",
                FontSize = 48,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            TextBlock instruction = new TextBlock
            {
                Text = "Thêm ảnh, scan hoặc PDF",
                FontSize = 14,
                Margin = new Thickness(0, 12, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            content.Children.Add(plusSign);
            content.Children.Add(instruction);
            grid.Children.Add(content);

            return grid;
        }

        // Tạo vùng preview file
        private Grid CreatePreviewArea()
        {
            Grid grid = new Grid();

            string ext = IOPath.GetExtension(_currentPage.FilePath).ToLower();

            if (ext == ".pdf")
            {
                // Hiển thị icon PDF
                StackPanel pdfIcon = new StackPanel
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                TextBlock icon = new TextBlock
                {
                    Text = "📄",
                    FontSize = 72,
                    HorizontalAlignment = HorizontalAlignment.Center
                };

                TextBlock fileName = new TextBlock
                {
                    Text = IOPath.GetFileName(_currentPage.FilePath),
                    FontSize = 14,
                    Margin = new Thickness(0, 16, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    MaxWidth = 400
                };

                pdfIcon.Children.Add(icon);
                pdfIcon.Children.Add(fileName);
                grid.Children.Add(pdfIcon);
            }
            else
            {
                // Hiển thị ảnh
                try
                {
                    Image image = new Image
                    {
                        Source = new BitmapImage(new Uri(_currentPage.FilePath)),
                        Stretch = Stretch.Uniform,
                        Margin = new Thickness(20)
                    };
                    grid.Children.Add(image);
                }
                catch
                {
                    TextBlock error = new TextBlock
                    {
                        Text = "Không thể hiển thị ảnh",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Foreground = Brushes.Red
                    };
                    grid.Children.Add(error);
                }
            }

            // Nút thay đổi file
            Button changeButton = new Button
            {
                Content = "Thay đổi file",
                Width = 120,
                Height = 35,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 0, 20),
                Cursor = Cursors.Hand
            };
            changeButton.Click += (s, e) => ChonFile();
            grid.Children.Add(changeButton);

            return grid;
        }

        // Chọn file
        private void ChonFile()
        {
            if (_currentPage == null) return;

            // Nếu đã có file, hỏi xác nhận
            if (!string.IsNullOrEmpty(_currentPage.FilePath))
            {
                var result = MessageBox.Show(
                    "Bạn có muốn thay thế file hiện tại?",
                    "Xác nhận",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result != MessageBoxResult.Yes)
                    return;
            }

            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "PDF hoặc ảnh|*.pdf;*.png;*.jpg;*.jpeg;*.bmp;*.gif",
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                _currentPage.FilePath = dialog.FileName;

                // Cập nhật màu button để biết đã có file
                _currentPage.PageButton.Foreground = Brushes.Green;
                _currentPage.PageButton.FontWeight = FontWeights.Bold;

                DisplayCurrentPage();

                MessageBox.Show(
                    "Đã thêm tài liệu thành công!",
                    "Thành công",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        // Xóa trang
        private void DeletePage(PageInfo page)
        {
            if (_pages.Count == 1)
            {
                MessageBox.Show(
                    "Không thể xóa trang cuối cùng!",
                    "Thông báo",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa trang {page.PageNumber}?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Xóa button khỏi panel
                PageListPanel.Children.Remove(page.PageButton);
                _pages.Remove(page);

                // Cập nhật lại số trang
                for (int i = 0; i < _pages.Count; i++)
                {
                    _pages[i].PageNumber = i + 1;
                    _pages[i].PageButton.Content = (i + 1).ToString();
                }

                // Chuyển sang trang khác
                if (_currentPage == page)
                {
                    SwitchToPage(_pages.First());
                }
            }
        }

        // Lấy danh sách file đã thêm
        public List<string> GetUploadedFiles()
        {
            return _pages
                .Where(p => !string.IsNullOrEmpty(p.FilePath))
                .Select(p => p.FilePath)
                .ToList();
        }
    }
}