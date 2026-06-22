# 🎲 Monopoly REST API

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![EF Core](https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

Проєкт гри "Монополія" з клієнт-серверною архітектурою. Бекенд реалізовано у вигляді REST API з дотриманням **Clean Architecture**, принципів **SOLID** та сучасних патернів проєктування.

👉 **Клієнтський додаток (Бот):** [Monopoly Telegram Bot](https://github.com/Maksym-0/Monopoly-Bot)

## 🛠 Стек технологій
- **Мова та фреймворк:** C#, .NET 8, ASP.NET Core Web API
- **База даних:** PostgreSQL, Entity Framework Core (Code-First)
- **Автентифікація:** JWT (JSON Web Token) Bearer

---

## 🏗 Чиста Архітектура (Clean Architecture)

Проєкт розділений на незалежні збірки (шари) для забезпечення слабкої зв'язності:

- **`.API` (Presentation Layer):** Точка входу. Містить налаштування DI (`Program.cs`) та контролери, які лише приймають HTTP-запити та повертають уніфіковані відповіді.
- **`.Application` (Service Layer):** Розділ бізнес-логіки (напр. `AccountService`, `GameService`, `TradingService`). Сервіси отримують дані з БД, валідують операції, керують станом і формують DTO.
- **`.Core` (Domain Layer):** Ядро ігрової логіки. Тут інкапсульовано стан моделей (Гравець, Дошка, Клітинки) та правила їх взаємодії. Завдяки модифікатору `internal`, стан моделей надійно захищений від несанкціонованої зміни ззовні. Тут також розміщені контракти (інтерфейси) та загальна абстракція (абстрактні класи).
- **`.DataAccess.Postgres` (Infrastructure Layer):** Реалізує контракти БД (`IUnitOfWork`, `IRepository`). Інкапсулює EF Core `DbContext` та конфігурації `IEntityTypeConfiguration`, роблячи базу даних лише деталлю реалізації.

---

## 💎 Застосування принципів SOLID

<details>
<summary><b>S — Single Responsibility (Єдина відповідальність)</b></summary>
Логіка розподілена на вузькоспеціалізовані сервіси. Наприклад, логіка обміну майном повністю винесена в окремий <code>TradingService</code>. Робота з БД винесена в окрему збірку з використанням патернів Repository та Unit Of Work.
</details>

<details>
<summary><b>O — Open/Closed (Відкритість/Закритість)</b></summary>
Бізнес-логіка залежить від абстракцій. Наприклад, метод <code>MoveAsync</code> не знає, на яку саме клітинку стає гравець. Він викликає абстрактний метод <code>ApplyEffect()</code>. Щоб додати нову клітинку (напр. "Парки"), достатньо створити новий клас-спадкоємець та поліморфізмом додати власну реалізацію відповідного ефекту при переході на клітину, не змінюючи існуючий протестований код сервісу.
</details>

<details>
<summary><b>L — Liskov Substitution (Підстановка Лісков)</b></summary>
Для безлічі типів клітинок (<code>AmbulanceCell</code>, <code>CasinoCell</code>, <code>CompanyCell</code>) створено базовий абстрактний клас <code>Cell</code>. Будь-яка клітинка може бути підставлена замість базового класу <code>Cell</code> у циклі, і гра коректно застосує її унікальний ефект без порушення роботи системи.
</details>

<details>
<summary><b>I — Interface Segregation (Розділення інтерфейсів)</b></summary>
Замість одного перевантаженого інтерфейсу <code>ICompanyCell</code>, логіку розбито на дрібні контракти: <code>IOwnable</code>, <code>IUpgradable</code>, <code>IMoneyEater</code>. Це дозволяє специфічним клітинкам (наприклад, <code>LosingMoneyCell</code>) наслідувати лише <code>IMoneyEater</code> для сплати штрафів, не реалізовуючи непотрібну їм логіку купівлі.
</details>

<details>
<summary><b>D — Dependency Inversion (Інверсія залежностей)</b></summary>
Усі шари (сервіси, репозиторії) залежать від абстракцій (інтерфейсів). Життєвий цикл об'єктів контролюється через DI-контейнер: наприклад, <code>DbContext</code> та <code>IUnitOfWork</code> зареєстровані як <code>Scoped</code>. Це гарантує, що EF Core Change Tracker коректно відслідковує транзакції в межах одного HTTP-запиту, уникаючи колізій.
</details>

---

## 🧩 Використані патерни проєктування

### 🏭 Factory Pattern (Фабрика)
Для ініціалізації гри (що вимагає складної ієрархії: Гра -> Дошка -> Клітинки -> Монополії -> Гравці) створено систему фабрик (`GameFactory`, `BoardFactory`). Вони розміщені в межах шару `.Core` і мають ексклюзивний доступ до `internal` полів моделей, що зберігає інкапсуляцію. Назовні доступний лише метод, який повертає повністю готову структуру.

### 🛡 Result Pattern
Замість управління потоком виконання через дорогі (з точки зору продуктивності) винятки (`throw new Exception`), використано дженерік-обгортки:
- **`ServiceResponse<T>`**: передає між шарами Application та API об'єкт DTO, статус-код та булевий прапорець успіху.
- **`ApiResponse<T>`**: уніфікована відповідь, яка повертається на клієнт, роблячи API стандартизованим та передбачуваним.

### 📦 Unit of Work & Repository
Взаємодія з БД інкапсульована. Сервіси ніколи не викликають `DbContext` напряму. Замість цього вони отримують `IUnitOfWork`, який акумулює всі зміни через Change Tracker та репозиторії, і зберігає їх однією транзакцією через `SaveChangesAsync()`.

---

## 🚀 Як запустити локально

1. Клонуйте репозиторій:
   ```bash
   git clone https://github.com/Maksym-0/Monopoly-API.git
   ```

2. Оновіть рядок підключення до PostgreSQL (DefaultConnection) та секретний ключ JWT у файлі appsettings.json (проєкт .API) або User Secrets у файлі secrets.json.

3. Перейдіть до проєкту API та застосуйте міграції через PowerShell (Package Manager Console у Visual Studio):
   ```bash
   Update-Database
   ```

4. Запустіть проєкт:
   ```bash
   dotnet run
   ```
