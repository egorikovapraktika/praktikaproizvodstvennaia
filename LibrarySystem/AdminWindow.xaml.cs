using System;
using System.Data;
using System.Windows;

namespace LibrarySystem
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            LoadAllData();
            
            btnReturn.Click += BtnReturn_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnAddBook.Click += BtnAddBook_Click;
            btnLogout.Click += BtnLogout_Click;
        }

        private void LoadAllData()
        {
            dgBooks.ItemsSource = DatabaseHelper.GetAllBooks().DefaultView;
            dgReaders.ItemsSource = DatabaseHelper.GetAllReaders().DefaultView;
            dgLoans.ItemsSource = DatabaseHelper.GetAllLoans().DefaultView;
            
            // Статистика
            txtTotalBooks.Text = DatabaseHelper.GetTotalBooks().ToString();
            txtTotalReaders.Text = DatabaseHelper.GetTotalReaders().ToString();
            txtActiveLoans.Text = DatabaseHelper.GetActiveLoans().ToString();
            txtOverdue.Text = DatabaseHelper.GetOverdueLoans().ToString();
        }

        private void BtnReturn_Click(object sender, RoutedEventArgs e)
        {
            if (dgLoans.SelectedItem == null)
            {
                MessageBox.Show("Выберите выдачу в списке!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            DataRowView row = (DataRowView)dgLoans.SelectedItem;
            int loanId = Convert.ToInt32(row["loan_id"]);
            
            if (DatabaseHelper.ReturnBook(loanId))
            {
                MessageBox.Show("Книга возвращена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadAllData();
            }
            else
            {
                MessageBox.Show("Ошибка при возврате!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddBook_Click(object sender, RoutedEventArgs e)
        {
            string title = txtNewBookTitle.Text.Trim();
            string author = txtNewBookAuthor.Text.Trim();
            int copies = int.TryParse(txtNewBookCopies.Text, out int c) ? c : 1;
            
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Введите название книги!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            if (DatabaseHelper.AddNewBook(title, author, copies))
            {
                MessageBox.Show($"Книга \"{title}\" добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                txtNewBookTitle.Text = "";
                txtNewBookAuthor.Text = "";
                txtNewBookCopies.Text = "1";
                LoadAllData();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadAllData();
            MessageBox.Show("Данные обновлены!", "Обновление", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}