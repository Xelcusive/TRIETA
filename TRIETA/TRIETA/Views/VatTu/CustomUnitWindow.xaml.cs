using System.Windows;
using System.Windows.Controls;

namespace Trita
{
    public partial class CustomUnitWindow : Window
    {
        public string UnitName { get; private set; }

        public CustomUnitWindow(string title = "Định nghĩa đơn vị", string prompt = "Nhập đơn vị mới:")
        {
            InitializeComponent();
            this.Title = title;

            // Cập nhật text prompt nếu tìm thấy TextBlock
            var contentBorder = this.Content as Border;
            if (contentBorder != null)
            {
                var stackPanel = contentBorder.Child as StackPanel;
                if (stackPanel != null && stackPanel.Children.Count > 0)
                {
                    var firstPanel = stackPanel.Children[0] as StackPanel;
                    if (firstPanel != null && firstPanel.Children.Count > 0)
                    {
                        var textBlock = firstPanel.Children[0] as TextBlock;
                        if (textBlock != null)
                        {
                            textBlock.Text = prompt;
                        }
                    }
                }
            }

            UnitTextBox.Focus();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(UnitTextBox.Text))
            {
                UnitName = UnitTextBox.Text.Trim();
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("Vui lòng nhập tên đơn vị!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                UnitTextBox.Focus();
            }
        }

        //private void Cancel_Click(object sender, RoutedEventArgs e)
        //{
        //    this.DialogResult = false;
        //    this.Close();
        //}
    }
}