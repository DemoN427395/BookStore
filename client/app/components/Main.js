'use client';

import React, { useEffect, useState } from 'react';
import Image from 'next/image';
import styles from './Main.module.css';

const Main = () => {
  const [books, setBooks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const controller = new AbortController();
    
    const fetchBooks = async () => {
      try {
        const response = await fetch('http://localhost:5001/api/books', {
          signal: controller.signal
        });
        
        if (!response.ok) throw new Error('Ошибка при загрузке данных');
        const data = await response.json();
        setBooks(data);
      } catch (error) {
        if (error.name !== 'AbortError') {
          setError(error.message);
        }
      } finally {
        setLoading(false);
      }
    };

    fetchBooks();
    return () => controller.abort();
  }, []);

  if (loading) return <div className={styles.loading}>Загрузка...</div>;
  if (error) return <div className={styles.error}>Ошибка: {error}</div>;

  return (
    <div className={styles.mainContainer}>
      <h1 className={styles.title}>Главная</h1>
      {books.length > 0 ? (
        <div className={styles.booksGrid}>
          {books.slice(0, 5).map((book) => (
            <div key={book.id} className={styles.bookCard}>
              <div className={styles.imageWrapper}>
                {book.imageUrl ? (
                  <Image
                    src={book.imageUrl}
                    alt={book.title}
                    width={200}
                    height={300}
                    className={styles.bookImage}
                  />
                ) : (
                  <div className={styles.placeholderImage}>
                    Изображение отсутствует
                  </div>
                )}
              </div>
              <h2 className={styles.bookTitle}>{book.title}</h2>
              <p className={styles.bookAuthor}>{book.author}</p>
            </div>
          ))}
        </div>
      ) : (
        <p className={styles.empty}>Нет доступных книг</p>
      )}
    </div>
  );
};

export default Main;