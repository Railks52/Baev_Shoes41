using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Baev_Shoes41
{
    public partial class OrderWindow : Window
    {
        private List<OrderProduct> selectedOrderProducts;
        private List<Product> selectedProducts;
        private User currentUser;
        private Order currentOrder = new Order();

        public OrderWindow(List<OrderProduct> selectedOrderProducts, List<Product> selectedProducts, User user)
        {
            InitializeComponent();

            this.selectedOrderProducts = selectedOrderProducts;
            this.selectedProducts = selectedProducts;
            this.currentUser = user;

            PickupCombo.ItemsSource = Baev_Shoes41Entities.GetContext().PickupPoint.ToList();

            if (currentUser != null)
                ClientTB.Text = $"{currentUser.UserSurname} {currentUser.UserName} {currentUser.UserPatronymic}";
            else
                ClientTB.Text = "Гость";

            TBOrderID.Text = selectedOrderProducts.First().OrderID.ToString();

            foreach (Product p in selectedProducts)
            {
                var op = selectedOrderProducts.FirstOrDefault(q => q.ProductArticleNumber == p.ProductArticleNumber);
                p.Quantity = op != null ? op.ProductAmount : 1;
            }

            ShoeListView.ItemsSource = selectedProducts;
            OrderDP.SelectedDate = DateTime.Now;

            UpdateOrderInfo();
        }

        private void UpdateOrderInfo()
        {
            decimal totalCost = 0;
            decimal totalDiscountSum = 0;
            bool allInStockMoreThan3 = true;

            foreach (var prod in selectedProducts)
            {
                decimal itemTotal = prod.ProductCost * prod.Quantity;
                decimal discountAmount = itemTotal * ((prod.ProductDiscountAmount ?? 0) / 100m);

                totalCost += itemTotal - discountAmount;
                totalDiscountSum += discountAmount;

                // Если на складе <= 3 позиций или меньше, чем заказано — срок доставки 6 дней
                if (prod.ProductQuantityInStock <= 3 || prod.ProductQuantityInStock < prod.Quantity)
                {
                    allInStockMoreThan3 = false;
                }
            }

            TotalSumTB.Text = $"Сумма заказа: {totalCost:F2} руб.";
            TotalDiscountTB.Text = $"Скидка: {totalDiscountSum:F2} руб.";

            int deliveryDays = allInStockMoreThan3 ? 3 : 6;
            DeliveryDP.SelectedDate = DateTime.Now.AddDays(deliveryDays);
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity++;

            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);
            if (selectedOP != null)
                selectedOP.ProductAmount++;

            UpdateOrderInfo();
            ShoeListView.Items.Refresh();
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            var prod = (sender as Button).DataContext as Product;
            prod.Quantity--;

            var selectedOP = selectedOrderProducts.FirstOrDefault(p => p.ProductArticleNumber == prod.ProductArticleNumber);

            if (prod.Quantity <= 0)
            {
                selectedProducts.Remove(prod);
                if (selectedOP != null)
                    selectedOrderProducts.Remove(selectedOP);

                if (selectedProducts.Count == 0)
                {
                    MessageBox.Show("Все товары удалены из заказа.");
                    this.Close();
                    return;
                }
            }
            else if (selectedOP != null)
            {
                selectedOP.ProductAmount--;
            }

            UpdateOrderInfo();
            ShoeListView.Items.Refresh();
        }
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PickupCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите пункт выдачи!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                currentOrder.OrderDate = OrderDP.SelectedDate.Value;
                currentOrder.OrderDeliveryDate = DeliveryDP.SelectedDate.Value;
                currentOrder.OrderPickupPoint = (PickupCombo.SelectedItem as PickupPoint).PickupPointID;
                currentOrder.OrderReceiptCode = new Random().Next(100, 999);
                currentOrder.OrderStatus = "Новый";

                if (currentUser != null)
                    currentOrder.OrderUser = currentUser.UserID;
                else
                    currentOrder.OrderUser = null;

                Baev_Shoes41Entities.GetContext().Order.Add(currentOrder);

                foreach (var op in selectedOrderProducts)
                {
                    Baev_Shoes41Entities.GetContext().OrderProduct.Add(op);
                }

                Baev_Shoes41Entities.GetContext().SaveChanges();
                MessageBox.Show($"Заказ №{currentOrder.OrderID} успешно оформлен! Код получения: {currentOrder.OrderReceiptCode}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                selectedProducts.Clear();
                selectedOrderProducts.Clear();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сохранения: {ex.InnerException?.Message ?? ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}