# Руководство по подключению Git к новому проекту

## Предварительные требования
- Установленный Git
- Аккаунт на GitHub
- Visual Studio
- Токен GitHub (если текущий истек)

## 1. Создание токена GitHub (если нужен новый)
1. Зайти на GitHub -> Ваш профиль -> Settings
2. Внизу слева: Developer settings -> Personal access tokens -> Tokens (classic)
3. Generate new token (classic)
4. Настройки токена:
   - Note: "VS Git access token"
   - Expiration: выбрать срок действия
   - Select scopes: отметить только `repo`
5. Нажать Generate token
6. **Важно**: Сохранить токен - показывается только один раз

## 2. Создание репозитория на GitHub
1. На GitHub: New repository
2. Заполнить:
   - Repository name: имя проекта
   - Description (опционально)
   - Public/Private
3. Не инициализировать с README
4. Скопировать URL репозитория

## 3. Настройка в Visual Studio
### Вариант 1: Новый проект
1. Создать новый проект в Visual Studio
2. В Team Explorer:
   - Нажать "Git Repository"
   - Initialize Git repository

### Вариант 2: Существующий проект
1. Открыть проект
2. В Team Explorer:
   - Changes
   - Initialize Git repository

## 4. Подключение к GitHub
В терминале Visual Studio выполнить команды:
```bash
# Инициализация Git (если еще не сделано)
& 'C:\Program Files\Git\cmd\git.exe' init

# Добавление удаленного репозитория
& 'C:\Program Files\Git\cmd\git.exe' remote add origin "https://github.com/ваш-username/название-репозитория.git"

# Добавление файлов
& 'C:\Program Files\Git\cmd\git.exe' add .

# Первый коммит
& 'C:\Program Files\Git\cmd\git.exe' commit -m "Initial commit"

# Отправка на GitHub
& 'C:\Program Files\Git\cmd\git.exe' push -f origin main
```

## Возможные проблемы и решения
1. Если push не проходит:
   ```bash
   & 'C:\Program Files\Git\cmd\git.exe' remote remove origin
   & 'C:\Program Files\Git\cmd\git.exe' remote add origin "https://github.com/ваш-username/название-репозитория.git"
   ```

2. При запросе учетных данных:
   - Username: ваш логин GitHub
   - Password: ваш токен GitHub

## Дальнейшая работа
После настройки для обычной работы достаточно:
```bash
git add .
git commit -m "описание изменений"
git push
```

## Важные замечания
1. Храните токен в безопасном месте
2. Не забывайте про срок действия токена
3. Используйте осмысленные commit-сообщения
4. Регулярно делайте push изменений

## Полезные ссылки
- [GitHub Documentation](https://docs.github.com)
- [Git Documentation](https://git-scm.com/doc)
- [Visual Studio Git Tutorial](https://docs.microsoft.com/en-us/azure/devops/repos/git/gitquickstart)
