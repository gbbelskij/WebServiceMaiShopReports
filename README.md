# Микросервис Генерации Отчетов

Этот микросервис генерирует ежемесячные отчеты о продажах в формате PDF на основе истории продаж пользователя. Сервис использует JWT для аутентификации, PostgreSQL для хранения данных и Swagger для документации API.

## Особенности

- **JWT Аутентификация**: Безопасные конечные точки API с использованием JSON Web Tokens.
- **База данных PostgreSQL**: Хранение данных о продажах в реляционной базе данных.
- **Документация Swagger**: Интерактивная документация и тестирование API.
- **PDF Отчеты**: Генерация подробных отчетов о продажах в формате PDF.

## Необходимые компоненты

- .NET 6.0 или новее
- PostgreSQL
- Docker (опционально, для контейнеризованного развёртывания)

## Начало работы

### Клонирование репозитория

```sh
git clone https://github.com/gbbelskij/WebServiceMaiShopReports.git
cd WebServiceMaiShopReports
```

### Настройка базы данных
```
"ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=YourDatabase;Username=YourUsername;Password=YourPassword"
}
```

### Запуск приложения
```
dotnet restore
dotnet build
dotnet run
```
Откройте браузер и перейдите на http://localhost:5000/swagger, чтобы получить доступ к документации Swagger.
