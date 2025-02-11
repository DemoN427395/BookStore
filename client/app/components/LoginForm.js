// 'use client';

// import { useState, useEffect } from 'react';
// import { useRouter } from 'next/navigation';

// export default function LoginForm() {
//   const [email, setEmail] = useState('');
//   const [password, setPassword] = useState('');
//   const [error, setError] = useState(null);
//   const [loading, setLoading] = useState(false);
//   const router = useRouter();

//   // Хук для проверки, когда приложение загружено в браузере
//   useEffect(() => {
//     if (typeof window !== 'undefined') {
//       // Если токен есть, редиректим на страницу рекомендаций
//       if (localStorage.getItem('token')) {
//         router.push('/recommendations');
//       }
//     }
//   }, [router]);

//   const handleLogin = async (e) => {
//     e.preventDefault();
//     setError(null);
//     setLoading(true);

//     try {
//       const response = await fetch('http://localhost:5000/api/auth/login', {
//         method: 'POST',
//         headers: { 'Content-Type': 'application/json' },
//         body: JSON.stringify({ email, password }),
//       });

//       const data = await response.json();
//       setLoading(false);

//       if (!response.ok) {
//         throw new Error(data.message || 'Ошибка авторизации');
//       }

//       if (!data.accessToken) {
//         throw new Error('Ошибка авторизации');
//       } else {
//         localStorage.setItem('token', data.accessToken);
//         console.log(localStorage.getItem('token'));
//         router.push('/recommendations');
//       }
//     } catch (err) {
//       setLoading(false);
//       setError(err.message);
//     }
//   };

//   return (
//     <div className="flex flex-col items-center justify-center h-screen bg-gray-100">
//       <form
//         onSubmit={handleLogin}
//         className="flex flex-col p-6 bg-white rounded-lg shadow-md w-80 space-y-4"
//       >
//         <h2 className="text-xl font-semibold text-center">Вход</h2>
//         {error && <p className="text-red-500 text-sm text-center">{error}</p>}

//         <div className="flex flex-col w-full">
//           <label className="mb-1 text-sm font-medium">Email</label>
//           <input
//             type="email"
//             placeholder="Введите email"
//             value={email}
//             onChange={(e) => setEmail(e.target.value)}
//             className="w-full p-2 border rounded"
//             required
//           />
//         </div>

//         <div className="flex flex-col w-full">
//           <label className="mb-1 text-sm font-medium">Пароль</label>
//           <input
//             type="password"
//             placeholder="Введите пароль"
//             value={password}
//             onChange={(e) => setPassword(e.target.value)}
//             className="w-full p-2 border rounded"
//             required
//           />
//         </div>

//         <button
//           type="submit"
//           className="w-full p-2 bg-blue-500 text-white rounded hover:bg-blue-600"
//           disabled={loading}
//         >
//           {loading ? 'Вход...' : 'Войти'}
//         </button>
//       </form>
//     </div>
//   );
// }
