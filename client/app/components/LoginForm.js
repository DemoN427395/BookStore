'use client';

import { useState } from "react";

export default function AuthForm() {
  const [name, setName] = useState(""); // состояние для имени (только для регистрации)
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState(null);
  const [isRegistering, setIsRegistering] = useState(false);

  // Обработчик для входа
  const handleLogin = async (e) => {
    e.preventDefault();
    setError(null);
    
    try {
      const response = await fetch("http://localhost:5241/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username: email, password }), // отправляем username как email
      });
      
      const data = await response.json();
      if (!response.ok) {
        throw new Error(data.message || "Ошибка авторизации");
      }
      
      console.log("Авторизация успешна!");
      console.log(`accessToken:${data.accessToken}, refreshToken:${data.refreshToken}`);
    } catch (err) {
      setError(err.message);
    }
  };

  // Обработчик для регистрации
  const handleRegister = async (e) => {
    e.preventDefault();
    setError(null);

    // Проверка на совпадение паролей
    if (password !== confirmPassword) {
      setError("Пароли не совпадают");
      return;
    }
    
    try {
      const response = await fetch("http://localhost:5241/api/auth/signup", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        // Отправляем необходимые поля: name, email и password
        body: JSON.stringify({ name, email, password }),
      });

      console.log(response);
      
      const data = await response.json();
      if (!response.ok) {
        throw new Error(data.message || "Ошибка регистрации");
      }
      
      console.log("Регистрация успешна!");
      console.log(data.message);
      // После успешной регистрации можно переключить форму на вход
      setIsRegistering(false);
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="flex flex-col items-center justify-center h-screen bg-gray-100">
      <form
        onSubmit={isRegistering ? handleRegister : handleLogin}
        className="flex flex-col p-6 bg-white rounded-lg shadow-md w-80 space-y-4"
      >
        <h2 className="text-xl font-semibold text-center">
          {isRegistering ? "Регистрация" : "Вход"}
        </h2>
        {error && <p className="text-red-500 text-sm text-center">{error}</p>}
        
        {/* Если регистрация, показываем поле для имени */}
        {isRegistering && (
          <div className="flex flex-col w-full">
            <label className="mb-1 text-sm font-medium">Имя</label>
            <input
              type="text"
              placeholder="Введите имя"
              value={name}
              onChange={(e) => setName(e.target.value)}
              className="w-full p-2 border rounded"
              required
            />
          </div>
        )}

        <div className="flex flex-col w-full">
          <label className="mb-1 text-sm font-medium">Email</label>
          <input
            type="email"
            placeholder="Введите email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            className="w-full p-2 border rounded"
            required
          />
        </div>

        <div className="flex flex-col w-full">
          <label className="mb-1 text-sm font-medium">Пароль</label>
          <input
            type="password"
            placeholder="Введите пароль"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            className="w-full p-2 border rounded"
            required
          />
        </div>

        {/* Поле подтверждения пароля показываем только при регистрации */}
        {isRegistering && (
          <div className="flex flex-col w-full">
            <label className="mb-1 text-sm font-medium">Подтверждение пароля</label>
            <input
              type="password"
              placeholder="Подтвердите пароль"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              className="w-full p-2 border rounded"
              required
            />
          </div>
        )}

        <button
          type="submit"
          className="w-full p-2 bg-blue-500 text-white rounded hover:bg-blue-600"
        >
          {isRegistering ? "Зарегистрироваться" : "Войти"}
        </button>
      </form>
      
      <div className="mt-4">
        <button
          onClick={() => {
            setError(null);
            setIsRegistering(!isRegistering);
          }}
          className="text-blue-500 hover:underline"
        >
          {isRegistering ? "Уже есть аккаунт? Войти" : "Нет аккаунта? Зарегистрироваться"}
        </button>
      </div>
    </div>
  );
}
