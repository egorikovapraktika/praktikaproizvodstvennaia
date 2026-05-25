using System;
using System.Windows;

namespace LibrarySystem
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string login = txtLogin.Text.Trim();
            string password = txtPassword.Password.Trim();

            if (login == "admin" && password == "admin123")
            {
                AdminWindow adminWin = new AdminWindow();
                adminWin.Show();
                this.Close();
                return;
            }

            int userId = DatabaseHelper.CheckUser(login, password);
            if (userId > 0)
            {
                UserWindow userWin = new UserWindow(userId);
                userWin.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль!", "Ошибка", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow regWin = new RegisterWindow();
            regWin.Show();
            this.Close();
        }
    }
}