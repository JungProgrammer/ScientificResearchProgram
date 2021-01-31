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
using System.Windows.Shapes;

namespace ResearchProgram
{
    /// <summary>
    /// Логика взаимодействия для CreateCustomerWindow.xaml
    /// </summary>
    public partial class CreateCustomerWindow : Window
    {
        bool _isEditCustomer = false;
        int _customerId = -1;
        string _customerTitle = "";
        public CreateCustomerWindow(Customer CustomerToEdit = null)
        {
            InitializeComponent();
            if (CustomerToEdit != null)
            {
                DeleteCustomerButton.Visibility = System.Windows.Visibility.Visible;
                _isEditCustomer = true;
                _customerTitle = CustomerToEdit.Title;
                _customerId = CustomerToEdit.Id;
                Title = "Редактирование заказчика";
                NewCustomerTextBox.Text = CustomerToEdit.Title;
                AddCustomerButton.Content = "Редактировать";
            }
        }

        private void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewCustomerTextBox.Text != "")
            {
                Customer customer = new Customer();
                customer.Title = NewCustomerTextBox.Text;
                if (_isEditCustomer)
                {
                    customer.Id = _customerId;
                    CRUDDataBase.UpdateCustomer(customer);
                    MessageBox.Show("Редактирование успешно ", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
                else
                {
                    CRUDDataBase.AddNewCustomer(customer);
                    MessageBox.Show("Добавление успешно ", "Успешно", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                }
            }
        }
        private void DeleteCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult sure = MessageBox.Show("Удалить заказчика " + _customerTitle + "?", "Удаление заказчика", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

            switch (sure)
            {
                case MessageBoxResult.Yes:
                    CRUDDataBase.DeleteCustomer(_customerId);
                    MessageBox.Show("Удаление успешно", "Удаление заказчика", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                    break;
            }
        }
    }
}
