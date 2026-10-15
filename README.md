# Web-приложение «Кадровое агентство»

ASP.NET Core MVC (.NET 10) + EF Core + xUnit. Поддерживаются **SQL Server** и **MySQL** через адаптер провайдера.

## Быстрый старт (SQL Server в Docker)

1. Установите [Docker Desktop](https://www.docker.com/products/docker-desktop/) и [.NET 10 SDK](https://dotnet.microsoft.com/download).
2. Поднимите MS SQL Server:

```bash
docker compose up -d
```

Дождитесь healthy-статуса контейнера `agency-mssql` (`docker compose ps`).

3. Запустите приложение:

```bash
dotnet restore
dotnet run --project Kursach.Web
```

При первом старте: миграции EF Core + сидинг (100+/10000+ записей).

Остановка БД:

```bash
docker compose down
```

Данные сохраняются в volume `agency_mssql_data`. Полный сброс:

```bash
docker compose down -v
```

### Параметры SQL Server

| Параметр | Значение по умолчанию |
|----------|------------------------|
| Порт | `1433` |
| Логин | `sa` |
| Пароль | `Agency_Strong_Pass123` |
| БД | `AgencyDb` (создаётся приложением) |

Переопределение: скопируйте `.env.example` → `.env` и измените `MSSQL_SA_PASSWORD` / `MSSQL_PORT` (пароль должен совпадать со строкой в `appsettings.json`).

Connection string:

```
Server=localhost,1433;Database=AgencyDb;User Id=sa;Password=Agency_Strong_Pass123;TrustServerCertificate=True;MultipleActiveResultSets=true
```

## Выбор СУБД

В `appsettings.json`:

```json
"Database": {
  "Provider": "SqlServer",
  "ConnectionStringName": "SqlServer"
}
```

Для MySQL:

```json
"Database": {
  "Provider": "MySql",
  "ConnectionStringName": "MySql"
}
```

Или:

```bash
dotnet run --project Kursach.Web -- --Database:Provider=MySql
```

| Ключ | Назначение |
|------|------------|
| `ConnectionStrings:SqlServer` | Docker / SQL Server |
| `ConnectionStrings:MySql` | MySQL |

Адаптеры: `IDatabaseProviderAdapter` → `SqlServerDatabaseAdapter` / `MySqlDatabaseAdapter`.

## Тесты

```bash
dotnet test
```

## Структура

- `docker-compose.yml` — MS SQL Server 2022
- `Kursach.Web` — MVC-приложение
- `Kursach.Tests` — модульные тесты

## Отчёты и доп. функции

- `/Applicants/MatchVacancies/{id}` — подбор вакансий для соискателя по навыкам
- `/Vacancies/OpenByIndustry` — открытые вакансии в выбранной сфере
- `/Vacancies/ClosedReport` — отчёт об успешно закрытых вакансиях
- `/Vacancies/Close/{id}` — закрытие вакансии с выбором принятого кандидата
