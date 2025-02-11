import { useEffect, useState } from 'react';
import Image from 'next/image';
import styles from './recommendations.module.css';
import { useRouter } from 'next/navigation';

export default function Recommendations({ books, token }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const router = useRouter();

  useEffect(() => {
    // Проверка токена в localStorage на клиентской стороне
    if (token || localStorage.getItem('token')) {
      setIsAuthenticated(true);
    } else {
      router.push('/');
    }
  }, [router, token]);

  if (!isAuthenticated) {
    // Пока идет проверка или если нет токена, можно показать индикатор загрузки
    return <div>Loading...</div>;
  }

  return (
    <div className={styles.container}>
      <h1 className={styles.title}>Рекомендации для вас</h1>
      <div className={styles.bookList}>
        {books.map((book) => (
          <div key={book.id} className={styles.bookItem}>
            <Image
              src={book.coverImageUrl}
              alt={book.title}
              width={150}
              height={220}
            />
            <h2>{book.title}</h2>
            <p>{book.author}</p>
          </div>
        ))}
      </div>
    </div>
  );
}

export async function getServerSideProps(context) {
  const token = context.req.cookies.token; // Считываем токен из cookies (если используется)

  // Если токен отсутствует, перенаправляем пользователя на страницу логина
  if (!token) {
    return {
      redirect: {
        destination: '/',
        permanent: false,
      },
    };
  }

  // Если токен есть, загружаем данные о книгах
  const res = await fetch('http://localhost:5001/api/books');
  const books = await res.json();

  return {
    props: {
      books,
      token,  // передаем токен на клиентскую сторону
    },
  };
}
