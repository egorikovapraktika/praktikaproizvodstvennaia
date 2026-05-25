using System;
using System.Windows;

namespace LibrarySystem
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            LoginWindow login = new LoginWindow();
            login.Show();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string lastName = txtLastName.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(firstName) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DatabaseHelper.RegisterUser(lastName, firstName, email, phone))
            {
                MessageBox.Show("Регистрация успешна!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Ошибка регистрации!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}