## Установка Postgre в Docker

Создайте .env:
```bash
POSTGRES_PASSWORD=your_password
```

```bash
docker-compose up -d
```

## Установка AuthTokenService

```bash
dotnet user-secrets init
dotnet user-secrets set "JWT:secret" "your-32-characters-long-super-strong-jwt-secret-key"
```

```bash
dotnet ef migrations add init
dotnet ef database update
```

## Установка UserService
```bash
dotnet ef migrations add init --context UserDataContext
dotnet ef database update --context UserDataContext
```

## **Описание API эндпоинтов:**

### AuthController

POST /api/auth/signup

Назначение: Регистрация нового пользователя.

Описание работы:

Проверяет, существует ли уже пользователь с данным Email.

Создаёт роль User (если отсутствует).

Регистрирует нового пользователя с указанными Email, именем и паролем.

Добавляет созданного пользователя в роль User.

Пример запроса (JSON):

`{
  "Email": "user@example.com",
  "Password": "YourPassword123!",
  "Name": "User Name"
}`

Ответ:

`201 Created – пользователь успешно зарегистрирован.`

`400 Bad Request – если пользователь уже существует или произошла ошибка при создании пользователя/роли.`

POST /api/auth/login

Назначение: Аутентификация пользователя и выдача JWT access-токена с refresh-токеном.

Описание работы:

Проверяет наличие пользователя по Email.

Валидирует пароль.

Формирует список клеймов (claims), включая имя пользователя и роли.

Генерирует JWT access-токен.

Генерирует refresh-токен и сохраняет (или обновляет) его в базе данных вместе с датой истечения срока действия.

Пример запроса (JSON):

`{
  "Email": "user@example.com",
  "Password": "YourPassword123!"
}`

Пример успешного ответа (JSON):

`{
  "AccessToken": "jwt_access_token",
  "RefreshToken": "refresh_token"
}`

Ответ:

`200 OK – успешная аутентификация.`

`400 Bad Request – если пользователь не найден.`

`401 Unauthorized – если пароль неверный.`

POST /api/auth/token/revoke

Назначение: Отзыв (аннулирование) refresh-токена для текущего аутентифицированного пользователя.

Требования: Заголовок Authorization: Bearer <jwt_access_token>

Описание работы:

Находит информацию о токене для текущего пользователя.
Обнуляет refresh-токен в базе данных.

Ответ:

`200 OK – если токен успешно отозван.`

`400 Bad Request – если данные пользователя не найдены.`

`500 Internal Server Error – при внутренней ошибке сервера.`

### UserController

GET /api/user

Назначение: Получение данных о текущем аутентифицированном пользователе.

Требования: Заголовок Authorization: Bearer <jwt_access_token>

Описание работы:

Извлекает имя пользователя из объекта HttpContext.User.

Возвращает сообщение с данными пользователя.

`{
  "Message": "User Data",
  "User": "user@example.com"
}`
