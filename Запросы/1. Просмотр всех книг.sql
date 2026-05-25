SELECT 
    b.book_id AS ID,
    b.title AS Книга,
    a.last_name + ' ' + a.first_name AS Автор,
    g.genre_name AS Жанр,
    b.publication_year AS Год,
    b.publisher AS Издательство,
    b.total_copies AS Всего,
    b.available_copies AS Доступно
FROM books b
LEFT JOIN book_authors ba ON b.book_id = ba.book_id
LEFT JOIN authors a ON ba.author_id = a.author_id
LEFT JOIN genres g ON b.genre_id = g.genre_id
ORDER BY b.title;