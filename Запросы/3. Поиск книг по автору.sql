DECLARE @author NVARCHAR(100) = N'Достоевский'; -- Введите фамилию автора

SELECT 
    b.title AS Книга,
    b.publication_year AS Год,
    g.genre_name AS Жанр,
    b.available_copies AS Доступно
FROM books b
JOIN book_authors ba ON b.book_id = ba.book_id
JOIN authors a ON ba.author_id = a.author_id
LEFT JOIN genres g ON b.genre_id = g.genre_id
WHERE a.last_name = @author
ORDER BY b.title;