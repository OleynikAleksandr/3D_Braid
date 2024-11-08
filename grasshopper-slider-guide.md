# Создание автоматических слайдеров в компонентах Grasshopper
## Ревизия 1 от 07.11.2024

## Описание проблемы
При стандартной разработке компонентов Grasshopper все числовые параметры требуют ручной настройки слайдеров пользователем:
- Создание слайдера
- Настройка пределов
- Установка значений по умолчанию
- Подключение к входу компонента
- Позиционирование на канвасе

Это создает неудобства при повторном использовании компонента, так как все настройки приходится выполнять заново.

## Найденное решение
В процессе разработки был обнаружен и отлажен способ автоматического создания предварительно настроенных слайдеров при добавлении компонента на канвас Grasshopper.

### Ключевые элементы решения:
1. Использование таймера для отложенного создания слайдера
2. Проверка инициализации всех необходимых атрибутов
3. Программное создание экземпляра GH_NumberSlider
4. Настройка всех параметров слайдера (пределы, значение по умолчанию, описание)
5. Автоматическое позиционирование и подключение к входному параметру

### Пример базовой реализации:
```csharp
public override void AddedToDocument(GH_Document document)
{
    base.AddedToDocument(document);
    
    Component = this;
    GrasshopperDocument = document;

    // Запускаем таймер для отложенного создания слайдера
    System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
    timer.Interval = 1000; // 1 секунда
    timer.Tick += (sender, e) =>
    {
        if (!_sliderCreated && this.Attributes != null && this.Params.Input[0].Attributes != null)
        {
            CreateSlider(document);
            _sliderCreated = true;
            timer.Stop();
            timer.Dispose();
        }
    };
    timer.Start();
}

private void CreateSlider(GH_Document document)
{
    Param_Number param = Params.Input[0] as Param_Number;
    if (param != null)
    {
        var slider = new GH_NumberSlider();
        slider.CreateAttributes();

        if (this.Attributes != null && this.Attributes.DocObject != null)
        {
            try
            {
                // Позиционирование слайдера
                slider.Attributes.Pivot = new PointF(
                    (float)this.Attributes.DocObject.Attributes.Bounds.Left - 200,
                    (float)param.Attributes.Bounds.Y
                );

                // Настройка параметров
                slider.Name = "Width (мм)";
                slider.NickName = "Width";
                slider.Description = "Ширина элемента (мм)";

                if (slider.Slider != null)
                {
                    slider.Slider.Type = GH_SliderAccuracy.Float;
                    slider.Slider.Minimum = Convert.ToDecimal(1.0);
                    slider.Slider.Maximum = Convert.ToDecimal(30.0);
                    slider.Slider.DecimalPlaces = 1;
                    slider.Slider.Value = Convert.ToDecimal(6.0);
                    slider.Slider.GripDisplay = GH_SliderGripDisplay.ShapeAndText;
                }

                // Добавление на канвас и подключение
                document.AddObject(slider, false);
                param.AddSource(slider);
                document.NewSolution(true);
            }
            catch (Exception ex)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, 
                    "Ошибка создания слайдера: " + ex.Message);
            }
        }
    }
}
```

## Важные моменты реализации

### 1. Почему нужен таймер
- Атрибуты компонента не сразу доступны после добавления на канвас
- Таймер позволяет дождаться полной инициализации компонента
- Проверяет готовность всех необходимых свойств перед созданием слайдера

### 2. Позиционирование слайдера
- Используем DocObject.Attributes.Bounds.Left для определения позиции компонента
- Отступ влево на 200 пикселей обеспечивает удобное расположение
- Y-координата берется из атрибутов входного параметра

### 3. Настройка параметров слайдера
```csharp
slider.Slider.Type = GH_SliderAccuracy.Float; // Тип значений
slider.Slider.DecimalPlaces = 1; // Количество десятичных знаков
slider.Slider.Minimum = Convert.ToDecimal(1.0); // Минимальное значение
slider.Slider.Maximum = Convert.ToDecimal(30.0); // Максимальное значение
slider.Slider.Value = Convert.ToDecimal(6.0); // Значение по умолчанию
```

### 4. Типы слайдеров
- Float: для чисел с плавающей точкой
- Integer: для целых чисел
- Even: только четные числа
- Odd: только нечетные числа

### 5. Отображение слайдера
```csharp
slider.Slider.GripDisplay = GH_SliderGripDisplay.ShapeAndText; // Стиль отображения
slider.Name = "Полное имя"; // Полное имя параметра
slider.NickName = "Краткое"; // Краткое имя
slider.Description = "Описание параметра"; // Всплывающая подсказка
```

## Возможные проблемы и решения

### 1. Слайдер не создается
- Проверьте все атрибуты на null перед использованием
- Увеличьте интервал таймера
- Добавьте отладочные сообщения

### 2. Неправильное позиционирование
- Проверьте значения Bounds
- Настройте отступы под ваши нужды
- Учитывайте масштаб интерфейса Grasshopper

### 3. Слайдер не подключается
- Убедитесь в совместимости типов
- Проверьте успешность AddSource
- Вызовите NewSolution после подключения

## Best Practices

1. Всегда проверяйте на null:
```csharp
if (this.Attributes?.DocObject?.Attributes != null)
{
    // Код создания слайдера
}
```

2. Используйте try-catch для обработки ошибок:
```csharp
try
{
    // Код создания слайдера
}
catch (Exception ex)
{
    this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, ex.Message);
}
```

3. Освобождайте ресурсы:
```csharp
timer.Stop();
timer.Dispose();
```

4. Используйте константы для настроек:
```csharp
private const float SLIDER_OFFSET_X = 200;
private const int TIMER_INTERVAL = 1000;
```

## Заключение
Данный подход позволяет создавать удобные предварительно настроенные компоненты с автоматическим позиционированием слайдеров. Это значительно улучшает пользовательский опыт и упрощает работу с компонентом.

## История изменений
- 07.11.2024 - Первая версия документа
- Найдено решение с использованием таймера
- Документированы основные подходы и решения проблем

