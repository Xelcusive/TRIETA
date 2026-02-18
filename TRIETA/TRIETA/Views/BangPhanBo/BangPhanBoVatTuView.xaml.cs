using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Trita.Views.VatTu; // ✅ Import để dùng VatTuItem và LoVatTu có sẵn

namespace Trita.Views.BangPhanBo
{
    public partial class BangPhanBoVatTuView : UserControl
    {
        // Chỉ định nghĩa các class riêng của BangPhanBo
        // KHÔNG định nghĩa lại VatTuItem và LoVatTu

        // Class lưu thông tin công tác sử dụng vật tư
        public class CongTac
        {
            public string? TenCongTac { get; set; }
            public string? LyTrinh { get; set; }
            public double KhoiLuongSuDung { get; set; }
            public string? MauSac { get; set; }
        }

        // Class cho segment của biểu đồ donut
        private class DonutSegment
        {
            public string? Label { get; set; }
            public double Value { get; set; }
            public double Percent { get; set; }
            public Color Color { get; set; }
        }

        private List<VatTuItem> danhSachVatTu=new();
        private VatTuItem? vatTuDangChon;
        private List<CongTac> danhSachCongTac=new();

        public BangPhanBoVatTuView()
        {
            InitializeComponent();
            KhoiTaoDuLieu();
        }

        // Lấy dữ liệu từ VatTuView
        public void LoadDataFromVatTuView(List<VatTuItem> danhSach)
        {
            danhSachVatTu = danhSach;
            HienThiDanhSachVatTu();
        }

        private void KhoiTaoDuLieu()
        {
            // Dữ liệu mẫu - trong thực tế sẽ lấy từ VatTuView
            danhSachVatTu = new List<VatTuItem>
            {
                new VatTuItem
                {
                    Ten = "Vật tư 1",
                    DonVi = "Tấn",
                    DanhSachLo = new List<LoVatTu>
                    {
                        new LoVatTu { KhoiLuong = 30 },
                        new LoVatTu { KhoiLuong = 28 }
                    }
                },
                new VatTuItem
                {
                    Ten = "Vật tư 2",
                    DonVi = "m³",
                    DanhSachLo = new List<LoVatTu>
                    {
                        new LoVatTu { KhoiLuong = 50 }
                    }
                },
                new VatTuItem
                {
                    Ten = "Vật tư 3",
                    DonVi = "kg",
                    DanhSachLo = new List<LoVatTu>
                    {
                        new LoVatTu { KhoiLuong = 100 },
                        new LoVatTu { KhoiLuong = 150 }
                    }
                }
            };

            // Dữ liệu công tác mẫu
            danhSachCongTac = new List<CongTac>
            {
                new CongTac { TenCongTac = "Công tác 1", LyTrinh = "Lý trình 1", KhoiLuongSuDung = 0, MauSac = "#FF6B35" },
                new CongTac { TenCongTac = "Công tác 2", LyTrinh = "Lý trình 2", KhoiLuongSuDung = 0, MauSac = "#FFA500" },
                new CongTac { TenCongTac = "Công tác 3", LyTrinh = "", KhoiLuongSuDung = 0, MauSac = "#FFD700" }
            };

            HienThiDanhSachVatTu();
        }

        private void HienThiDanhSachVatTu()
        {
            DanhSachVatTuPanel.Children.Clear();

            if (danhSachVatTu == null || danhSachVatTu.Count == 0)
            {
                TextBlock empty = new TextBlock
                {
                    Text = "Chưa có vật tư nào",
                    FontStyle = FontStyles.Italic,
                    Foreground = Brushes.Gray,
                    Margin = new Thickness(10)
                };
                DanhSachVatTuPanel.Children.Add(empty);
                return;
            }

            for (int i = 0; i < danhSachVatTu.Count; i++)
            {
                VatTuItem vt = danhSachVatTu[i];

                Button btnVatTu = new Button
                {
                    Content = $"{i + 1}.   [{vt.Ten}]",
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    Padding = new Thickness(5, 8, 5, 8),
                    FontWeight = FontWeights.Bold
                };

                btnVatTu.Click += (s, e) =>
                {
                    vatTuDangChon = vt;
                    HienThiBangPhanBo();
                };

                DanhSachVatTuPanel.Children.Add(btnVatTu);

                if (i < danhSachVatTu.Count - 1)
                {
                    TextBlock dots = new TextBlock
                    {
                        Text = ".......................",
                        Foreground = Brushes.Gray,
                        Margin = new Thickness(0, 2, 0, 2)
                    };
                    DanhSachVatTuPanel.Children.Add(dots);
                }
            }
        }

        private void HienThiBangPhanBo()
        {
            if (vatTuDangChon == null) return;

            TitleTextBlock.Text = vatTuDangChon.Ten.ToUpper();
            DonViTextBlock.Text = $"Đơn vị: [{vatTuDangChon.DonVi}]";

            double tongKhoiLuong = vatTuDangChon.DanhSachLo.Sum(lo => lo.KhoiLuong);
            double tongDaSuDung = danhSachCongTac.Sum(ct => ct.KhoiLuongSuDung);
            double conLai = tongKhoiLuong - tongDaSuDung;

            TongKhoiLuongTextBlock.Text = $"{conLai:F2}";

            HienThiBangCongTac(tongKhoiLuong);
            HienThiThongKeLo();
            VeBieuDoDonut(tongKhoiLuong, tongDaSuDung);
        }

        private void HienThiBangCongTac(double tongKhoiLuong)
        {
            BangPhanBoPanel.Children.Clear();

            Grid headerRow = TaoRowBang(
                "",
                "Vật tư đầu kỳ",
                "",
                "",
                tongKhoiLuong.ToString("F2"),
                true
            );
            headerRow.Background = new SolidColorBrush(Color.FromRgb(230, 240, 255));
            BangPhanBoPanel.Children.Add(headerRow);

            for (int i = 0; i < danhSachCongTac.Count; i++)
            {
                CongTac ct = danhSachCongTac[i];

                Grid row = TaoRowBang(
                    (i + 1).ToString(),
                    $"[{ct.TenCongTac}]",
                    $"[{ct.LyTrinh}]",
                    $"[{ct.KhoiLuongSuDung:F2}]",
                    $"[{ct.KhoiLuongSuDung:F2}]",
                    false
                );

                if (i % 2 == 0)
                {
                    row.Background = Brushes.White;
                }
                else
                {
                    row.Background = new SolidColorBrush(Color.FromRgb(249, 249, 249));
                }

                BangPhanBoPanel.Children.Add(row);
            }
        }

        private Grid TaoRowBang(string stt, string tenCongTac, string lyTrinh, string daSuDung, string tonDu, bool isHeader)
        {
            Grid row = new Grid { Height = 40 };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            FontWeight fontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal;

            TextBlock txtStt = new TextBlock
            {
                Text = stt,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = fontWeight,
                Foreground = isHeader ? Brushes.Red : Brushes.Black
            };
            Grid.SetColumn(txtStt, 0);

            TextBlock txtTen = new TextBlock
            {
                Text = tenCongTac,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = fontWeight,
                Foreground = isHeader ? Brushes.Red : Brushes.Black
            };
            Grid.SetColumn(txtTen, 1);

            TextBlock txtLyTrinh = new TextBlock
            {
                Text = lyTrinh,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = fontWeight,
                Foreground = isHeader ? Brushes.Red : Brushes.Black
            };
            Grid.SetColumn(txtLyTrinh, 2);

            TextBlock txtDaSuDung = new TextBlock
            {
                Text = daSuDung,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = fontWeight,
                Foreground = isHeader ? Brushes.Red : Brushes.Black
            };
            Grid.SetColumn(txtDaSuDung, 3);

            TextBlock txtTonDu = new TextBlock
            {
                Text = tonDu,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontWeight = fontWeight,
                Foreground = isHeader ? Brushes.Red : Brushes.Black
            };
            Grid.SetColumn(txtTonDu, 4);

            row.Children.Add(txtStt);
            row.Children.Add(txtTen);
            row.Children.Add(txtLyTrinh);
            row.Children.Add(txtDaSuDung);
            row.Children.Add(txtTonDu);

            for (int i = 0; i < 5; i++)
            {
                if (i > 0)
                {
                    Border border = new Border
                    {
                        BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                        BorderThickness = new Thickness(1, 0, 0, 0)
                    };
                    Grid.SetColumn(border, i);
                    row.Children.Add(border);
                }
            }

            Border bottomBorder = new Border
            {
                BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                BorderThickness = new Thickness(0, 0, 0, 1)
            };
            Grid.SetColumnSpan(bottomBorder, 5);
            row.Children.Add(bottomBorder);

            return row;
        }

        private void HienThiThongKeLo()
        {
            ThongKeLoPanel.Children.Clear();

            if (vatTuDangChon == null) return;

            for (int i = 0; i < vatTuDangChon.DanhSachLo.Count; i++)
            {
                LoVatTu lo = vatTuDangChon.DanhSachLo[i];

                Grid grid = new Grid { Margin = new Thickness(0, 4, 0, 4) };
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                Border labelBorder = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(230, 230, 230)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(15),
                    Height = 28
                };
                TextBlock label = new TextBlock
                {
                    Text = $"Lô {i + 1}",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontWeight = FontWeights.Bold,
                    FontSize = 11
                };
                labelBorder.Child = label;
                Grid.SetColumn(labelBorder, 0);

                Border valueBorder = new Border
                {
                    Background = Brushes.White,
                    BorderBrush = new SolidColorBrush(Color.FromRgb(204, 204, 204)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(15),
                    Height = 28,
                    Margin = new Thickness(8, 0, 0, 0)
                };
                TextBlock value = new TextBlock
                {
                    Text = $"[{lo.KhoiLuong:F2}]",
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 11
                };
                valueBorder.Child = value;
                Grid.SetColumn(valueBorder, 1);

                grid.Children.Add(labelBorder);
                grid.Children.Add(valueBorder);
                ThongKeLoPanel.Children.Add(grid);
            }
        }

        private void VeBieuDoDonut(double tongKhoiLuong, double tongDaSuDung)
        {
            DonutCanvas.Children.Clear();

            if (vatTuDangChon == null || tongKhoiLuong == 0) return;

            double centerX = 120;
            double centerY = 160;
            double outerRadius = 120;
            double innerRadius = 70;

            double conLai = tongKhoiLuong - tongDaSuDung;
            double percentConLai = (conLai / tongKhoiLuong) * 100;

            List<DonutSegment> segments = new List<DonutSegment>();

            segments.Add(new DonutSegment
            {
                Label = "Tồn kho",
                Value = conLai,
                Percent = percentConLai,
                Color = Color.FromRgb(70, 130, 180)
            });

            string[] colors = { "#FF6B35", "#FFA500", "#FFD700" };
            for (int i = 0; i < danhSachCongTac.Count; i++)
            {
                if (danhSachCongTac[i].KhoiLuongSuDung > 0)
                {
                    double percent = (danhSachCongTac[i].KhoiLuongSuDung / tongKhoiLuong) * 100;
                    Color color = (Color)ColorConverter.ConvertFromString(colors[i % colors.Length]);

                    segments.Add(new DonutSegment
                    {
                        Label = danhSachCongTac[i].TenCongTac,
                        Value = danhSachCongTac[i].KhoiLuongSuDung,
                        Percent = percent,
                        Color = color
                    });
                }
            }

            double startAngle = -90;

            foreach (var segment in segments)
            {
                double sweepAngle = (segment.Percent / 100) * 360;

                // Vẽ segment
                Path path = CreateDonutSegment(centerX, centerY, outerRadius, innerRadius, startAngle, sweepAngle);
                path.Fill = new SolidColorBrush(segment.Color);
                path.Stroke = Brushes.White;
                path.StrokeThickness = 2;

                DonutCanvas.Children.Add(path);

                // Tính vị trí để đặt % (giữa vòng tròn)
                double midAngle = startAngle + (sweepAngle / 2);
                double midRadius = (outerRadius + innerRadius) / 2;
                double radians = midAngle * Math.PI / 180;
                double textX = centerX + midRadius * Math.Cos(radians);
                double textY = centerY + midRadius * Math.Sin(radians);

                // Hiển thị % nếu segment đủ lớn (>5%)
                if (segment.Percent > 5)
                {
                    TextBlock percentText = new TextBlock
                    {
                        Text = $"{segment.Percent:F0}%",
                        FontSize = 14,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.White,
                        TextAlignment = TextAlignment.Center
                    };

                    percentText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Canvas.SetLeft(percentText, textX - percentText.DesiredSize.Width / 2);
                    Canvas.SetTop(percentText, textY - percentText.DesiredSize.Height / 2);
                    DonutCanvas.Children.Add(percentText);
                }

                startAngle += sweepAngle;
            }

            // Vẽ chú thích màu bên dưới biểu đồ
            VeLegend(segments, centerY + outerRadius + 20);
        }

        private void VeLegend(List<DonutSegment> segments, double startY)
        {
            double offsetY = 0;

            foreach (var segment in segments)
            {
                StackPanel legendItem = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5, 3, 5, 3)
                };

                // Ô màu
                Rectangle colorBox = new Rectangle
                {
                    Width = 16,
                    Height = 16,
                    Fill = new SolidColorBrush(segment.Color),
                    Margin = new Thickness(0, 0, 10, 0),
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Text
                TextBlock text = new TextBlock
                {
                    Text = segment.Label,
                    FontSize = 12,
                    FontWeight = FontWeights.Medium,
                    VerticalAlignment = VerticalAlignment.Center
                };

                legendItem.Children.Add(colorBox);
                legendItem.Children.Add(text);

                Canvas.SetLeft(legendItem, 20);
                Canvas.SetTop(legendItem, startY + offsetY);
                DonutCanvas.Children.Add(legendItem);

                offsetY += 28;
            }
        }

        private Path CreateDonutSegment(double cx, double cy, double outerR, double innerR, double startAngle, double sweepAngle)
        {
            if (sweepAngle >= 360)
            {
                sweepAngle = 359.999;
            }

            double startRad = startAngle * Math.PI / 180;
            double endRad = (startAngle + sweepAngle) * Math.PI / 180;

            Point p1 = new Point(cx + outerR * Math.Cos(startRad), cy + outerR * Math.Sin(startRad));
            Point p2 = new Point(cx + outerR * Math.Cos(endRad), cy + outerR * Math.Sin(endRad));
            Point p3 = new Point(cx + innerR * Math.Cos(endRad), cy + innerR * Math.Sin(endRad));
            Point p4 = new Point(cx + innerR * Math.Cos(startRad), cy + innerR * Math.Sin(startRad));

            bool largeArc = sweepAngle > 180;

            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure { StartPoint = p1 };

            figure.Segments.Add(new ArcSegment
            {
                Point = p2,
                Size = new Size(outerR, outerR),
                IsLargeArc = largeArc,
                SweepDirection = SweepDirection.Clockwise
            });

            figure.Segments.Add(new LineSegment { Point = p3 });

            figure.Segments.Add(new ArcSegment
            {
                Point = p4,
                Size = new Size(innerR, innerR),
                IsLargeArc = largeArc,
                SweepDirection = SweepDirection.Counterclockwise
            });

            figure.IsClosed = true;
            geometry.Figures.Add(figure);

            return new Path { Data = geometry };
        }
    }
}