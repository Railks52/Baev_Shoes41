using System;
using System.Collections.Generic;
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

namespace Baev_Shoes41
{
    /// <summary>
    /// Логика взаимодействия для ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        private void UpdateProduct()
        {
            var allProducts = Baev_Shoes41Entities.GetContext().Product.ToList();
            int totalCount = allProducts.Count;

            var currentProducts = allProducts;

            if (ComboType.SelectedIndex == 1)
                currentProducts = currentProducts.Where(p => p.ProductDiscountAmount >= 0 && p.ProductDiscountAmount < 10).ToList();

            if (ComboType.SelectedIndex == 2)
                currentProducts = currentProducts.Where(p => p.ProductDiscountAmount >= 10 && p.ProductDiscountAmount < 15).ToList();

            if (ComboType.SelectedIndex == 3)
                currentProducts = currentProducts.Where(p => p.ProductDiscountAmount >= 15 && p.ProductDiscountAmount <= 100).ToList();

            if (!string.IsNullOrWhiteSpace(TBoxSearch.Text))
            {
                currentProducts = currentProducts.Where(p => p.ProductName.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            }

            if (RButtonDown.IsChecked == true)
                currentProducts = currentProducts.OrderByDescending(p => p.ProductCost).ToList();
            
            if (RButtonUp.IsChecked == true)
                currentProducts = currentProducts.OrderBy(p => p.ProductCost).ToList();

            ProductListView.ItemsSource = currentProducts;
            TBCount.Text = $"Кол-во {currentProducts.Count} из {totalCount}";
        }
        public ProductPage(User user)
        {
            InitializeComponent();
            if (user != null)
            {
                TBAuth.Text = $"Вы авторизованы как {user.UserSurname} {user.UserName} {user.UserPatronymic}";
                TBRole.Text = $"Роль: " + Baev_Shoes41Entities.GetContext().Role.Find(user.UserRole).RoleName.ToString();
            }
            else
            {
                TBAuth.Text = "Вы авторизованы как гость";
                TBRole.Text = "Роль: гость";
            }

            var currentProducts = Baev_Shoes41Entities.GetContext().Product.ToList();
            ProductListView.ItemsSource = currentProducts;

            ComboType.SelectedIndex = 0;
            UpdateProduct();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void ComboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateProduct();
        }

        private void RButtonUp_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
        }

        private void RButtonDown_Checked(object sender, RoutedEventArgs e)
        {
            UpdateProduct();
        }
    }
}
