using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace LibrarySystem
{
    public class DatabaseHelper
    {
        // ВАША СТРОКА ПОДКЛЮЧЕНИЯ - ИСПРАВЬТЕ
        private static string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=LibraryDB;Integrated Security=True;TrustServerCertificate=True;";
        
        // ============ ВХОД И РЕГИСТРАЦИЯ ============
        public static int CheckUser(string email, string phone)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT reader_id FROM readers WHERE email = @email AND phone = @phone";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : -1;
                }
            }
            catch { return -1; }
        }
        
        public static DataTable GetUserInfo(int userId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT last_name + ' ' + first_name AS name FROM readers WHERE reader_id = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", userId);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            catch { }
            return dt;
        }
        
        public static bool RegisterUser(string lastName, string firstName, string email, string phone)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO readers (last_name, first_name, email, phone, registration_date, is_active) VALUES (@ln, @fn, @email, @phone, GETDATE(), 1)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ln", lastName);
                    cmd.Parameters.AddWithValue("@fn", firstName);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch { return false; }
        }
        
        // ============ ЧИТАТЕЛЬ ============
        public static DataTable GetAvailableBooks()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT book_id, title, available_copies FROM books WHERE available_copies > 0";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.Fill(dt);
                }
            }
            catch { }
            return dt;
        }
        
        public static int GetTotalAvailableBooks()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT SUM(available_copies) FROM books";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToInt32(result) : 0;
                }
            }
            catch { return 0; }
        }
        
        public static DataTable GetMyBooks(int readerId)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT l.loan_id, b.title, l.loan_date, l.due_date, 
                                    DATEDIFF(DAY, l.due_date, GETDATE()) AS overdue
                                    FROM book_loans l 
                                    JOIN books b ON l.book_id = b.book_id 
                                    WHERE l.reader_id = @rid AND l.return_date IS NULL";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@rid", readerId);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            catch { }
            return dt;
        }
        
        public static bool BorrowBook(int bookId, int readerId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();
                    
                    string query1 = "INSERT INTO book_loans (book_id, reader_id, due_date) VALUES (@bid, @rid, DATEADD(DAY, 14, GETDATE()))";
                    SqlCommand cmd1 = new SqlCommand(query1, conn, transaction);
                    cmd1.Parameters.AddWithValue("@bid", bookId);
                    cmd1.Parameters.AddWithValue("@rid", readerId);
                    cmd1.ExecuteNonQuery();
                    
                    string query2 = "UPDATE books SET available_copies = available_copies - 1 WHERE book_id = @bid";
                    SqlCommand cmd2 = new SqlCommand(query2, conn, transaction);
                    cmd2.Parameters.AddWithValue("@bid", bookId);
                    cmd2.ExecuteNonQuery();
                    
                    transaction.Commit();
                    return true;
                }
            }
            catch { return false; }
        }
        
        // ============ АДМИНИСТРАТОР ============
        public static DataTable GetAllBooks()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT book_id, title, total_copies, available_copies FROM books";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.Fill(dt);
                }
            }
            catch { }
            return dt;
        }
        
        public static DataTable GetAllReaders()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT reader_id, last_name, first_name, email, phone FROM readers";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.Fill(dt);
                }
            }
            catch { }
            return dt;
        }
        
        public static DataTable GetAllLoans()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"SELECT l.loan_id, r.last_name + ' ' + r.first_name AS reader, 
                                    b.title AS book, l.loan_date, l.due_date,
                                    CASE WHEN l.return_date IS NULL AND l.due_date < GETDATE() THEN 'Просрочена'
                                         WHEN l.return_date IS NULL THEN 'На руках'
                                         ELSE 'Возвращена' END AS status
                                    FROM book_loans l 
                                    JOIN readers r ON l.reader_id = r.reader_id 
                                    JOIN books b ON l.book_id = b.book_id
                                    ORDER BY l.loan_date DESC";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    adapter.Fill(dt);
                }
            }
            catch { }
            return dt;
        }
        
        public static int GetTotalBooks()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM books";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { return 0; }
        }
        
        public static int GetTotalReaders()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM readers";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { return 0; }
        }
        
        public static int GetActiveLoans()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM book_loans WHERE return_date IS NULL";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { return 0; }
        }
        
        public static int GetOverdueLoans()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM book_loans WHERE return_date IS NULL AND due_date < GETDATE()";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch { return 0; }
        }
        
        public static bool ReturnBook(int loanId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction transaction = conn.BeginTransaction();
                    
                    string query1 = "UPDATE book_loans SET return_date = GETDATE(), is_returned = 1 WHERE loan_id = @lid";
                    SqlCommand cmd1 = new SqlCommand(query1, conn, transaction);
                    cmd1.Parameters.AddWithValue("@lid", loanId);
                    cmd1.ExecuteNonQuery();
                    
                    string query2 = "UPDATE books SET available_copies = available_copies + 1 WHERE book_id = (SELECT book_id FROM book_loans WHERE loan_id = @lid)";
                    SqlCommand cmd2 = new SqlCommand(query2, conn, transaction);
                    cmd2.Parameters.AddWithValue("@lid", loanId);
                    cmd2.ExecuteNonQuery();
                    
                    transaction.Commit();
                    return true;
                }
            }
            catch { return false; }
        }
        
        public static bool AddNewBook(string title, string author, int copies)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO books (title, total_copies, available_copies) VALUES (@title, @copies, @copies)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@title", title);
                    cmd.Parameters.AddWithValue("@copies", copies);
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch { return false; }
        }
    }
}