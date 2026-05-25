using System;
using System.Data;
using System.Windows;

namespace LibrarySystem
{
    public partial class UserWindow : Window
    {
        private int currentUserId;
        private string currentUserName;
        
        public UserWindow(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadUserInfo();
            LoadData();
            
            btnBorrow.Click += BtnBorrow_Click;
            btnRefresh.Click += BtnRefresh_Click;
            btnLogout.Click += BtnLogout_Click;
        }

        private void LoadUserInfo()
        {
            DataTable userInfo = DatabaseHelper.GetUserInfo(currentUserId);
            if (userInfo.Rows.Count > 0)
            {
                currentUserName = userInfo.Rows[0]["name"].ToString();
                txtUserName.Text = currentUserName;
            }
        }

        private void LoadData()
        {
            LoadAvailableBooks();
            LoadMyBooks();
            LoadStats();
        }

        private void LoadAvailableBooks()
        {
            dgBooks.ItemsSource = DatabaseHelper.GetAvailableBooks().DefaultView;
        }

        private void LoadMyBooks()
        {
            DataTable myBooks = DatabaseHelper.GetMyBooks(currentUserId);
            dgMyBooks.ItemsSource = myBooks.DefaultView;
            
            // Считаем просроченные
            int overdue = 0;
            foreach (DataRow row in myBooks.Rows)
            {
                if (row["overdue"] != DBNull.Value && Convert.ToInt32(row["overdue"]) > 0)
                    overdue++;
            }
            txtOverdue.Text = overdue.ToString();
            txtMyBooks.Text = myBooks.Rows.Count.ToString();
        }

        private void LoadStats()
        {
            int total = DatabaseHelper.GetTotalAvailableBooks();
            txtTotalBooks.Text = total.ToString();
        }

        private void BtnBorrow_Click(object sender, RoutedEventArgs e)
        {
            if (dgBooks.SelectedItem == null)
            {
                MessageBox.Show("Выберите книгу!", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            
            DataRowView row = (DataRowView)dgBooks.SelectedItem;
            int bookId = Convert.ToInt32(row["book_id"]);
            string bookTitle = row["title"].ToString();
            
            var result = MessageBox.Show($"Вы уверены, что хотите взять книгу:\n\n{bookTitle}\n\nна 14 дней?", 
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                if (DatabaseHelper.BorrowBook(bookId, currentUserId))
                {
                    MessageBox.Show($"Книга \"{bookTitle}\" выдана на 14 дней!", "Успех", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadData();
                }
                else
                {
                    MessageBox.Show("Не удалось выдать книгу!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
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