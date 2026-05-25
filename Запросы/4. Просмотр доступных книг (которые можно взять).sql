SELECT 
    b.title AS Книга,
    a.last_name + ' ' + a.first_name AS Автор,
    g.genre_name AS Жанр,
    b.available_copies AS Доступно_экземпляров
FROM books b
LEFT JOIN book_authors ba ON b.book_id = ba.book_id
LEFT JOIN authors a ON ba.author_id = a.author_id
LEFT JOIN genres g ON b.genre_id = g.genre_id
WHERE b.available_copies > 0
ORDER BY b.title;