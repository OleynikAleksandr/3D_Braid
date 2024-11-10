# Решение проблемы синхронного перемещения связанных объектов в Grasshopper
## Ревизия 3 от 10.11.2024

## Описание проблемы
При перемещении компонента Grasshopper связанные с ним объекты (слайдеры, параметры и т.д.) должны перемещаться вместе с ним, сохраняя относительное расположение.

## Найденное решение
Решение основано на использовании стандартных механизмов Grasshopper для отслеживания и обработки перемещений через класс атрибутов компонента.

### Ключевые элементы решения:
1. Переопределение класса атрибутов компонента (GH_ComponentAttributes)
2. Отслеживание изменений позиции через сравнение Pivot
3. Использование механизма отмены для корректной регистрации операций
4. Принудительное обновление документа после перемещения

### Пример реализации:
```csharp
public class CustomComponentAttributes : GH_ComponentAttributes
{
    private readonly CustomComponent owner;
    private PointF lastPosition;

    public CustomComponentAttributes(CustomComponent component)
        : base(component)
    {
        this.owner = component;
        this.lastPosition = Pivot;
    }

    protected override void Layout()
    {
        base.Layout();
        Rectangle bounds = GH_Convert.ToRectangle(Bounds);
        bounds.Height += 30;
        Bounds = bounds;

        if (lastPosition != Pivot)
        {
            float deltaX = Pivot.X - lastPosition.X;
            float deltaY = Pivot.Y - lastPosition.Y;

            // Записываем действие перемещения для возможности отмены
            owner.RecordUndoEvent("Move Objects");

            UpdateConnectedObjects(deltaX, deltaY);
            lastPosition = Pivot;

            // Обновляем документ
            GH_Document doc = owner.OnPingDocument();
            if (doc != null)
            {
                doc.DestroyAttributeCache();
                doc.ScheduleSolution(10);
            }
        }
    }

    private void UpdateConnectedObjects(float deltaX, float deltaY)
    {
        if (owner?.Params?.Input == null) return;

        foreach (IGH_Param param in owner.Params.Input)
        {
            if (param.Sources != null)
            {
                foreach (IGH_Param source in param.Sources)
                {
                    if (source?.Attributes != null)
                    {
                        var currentPivot = source.Attributes.Pivot;
                        source.Attributes.Pivot = new PointF(
                            currentPivot.X + deltaX,
                            currentPivot.Y + deltaY
                        );
                        source.Attributes.ExpireLayout();
                    }
                }
            }
        }
    }
}
```

## Важные аспекты реализации

### 1. Отслеживание позиции
```csharp
private PointF lastPosition;
// ...
if (lastPosition != Pivot)
{
    float deltaX = Pivot.X - lastPosition.X;
    float deltaY = Pivot.Y - lastPosition.Y;
    // ...
}
```
- Сохраняем последнюю известную позицию
- Сравниваем с текущей для определения перемещения
- Вычисляем дельту для корректного смещения связанных объектов

### 2. Регистрация действия отмены
```csharp
owner.RecordUndoEvent("Move Objects");
```
- Регистрирует перемещение как операцию, которую можно отменить
- Интегрирует перемещение в систему Undo/Redo Grasshopper
- Обеспечивает корректное обновление состояния документа

### 3. Обновление связанных объектов
```csharp
source.Attributes.Pivot = new PointF(
    currentPivot.X + deltaX,
    currentPivot.Y + deltaY
);
source.Attributes.ExpireLayout();
```
- Применяем то же смещение ко всем связанным объектам
- Обновляем layout для корректного отображения
- Сохраняем относительное расположение объектов

### 4. Обновление документа
```csharp
doc.DestroyAttributeCache();
doc.ScheduleSolution(10);
```
- Сбрасываем кэш атрибутов для принудительного обновления
- Планируем пересчет решения для обновления отображения
- Обеспечиваем корректное визуальное обновление

## Преимущества решения
1. Использует стандартные механизмы Grasshopper
2. Поддерживает отмену операций
3. Сохраняет целостность связей между объектами
4. Обеспечивает плавное визуальное обновление

## Ограничения
1. Работает только для прямых связей (источников параметров)
2. Требует корректной инициализации атрибутов
3. Может создавать дополнительную нагрузку при частых перемещениях

## Применение
Данный метод может быть использован в любых компонентах Grasshopper, где требуется синхронное перемещение связанных объектов:
1. Компоненты с предустановленными слайдерами
2. Компоненты с дополнительными параметрами
3. Компоненты с вложенными элементами управления

## История изменений
- 10.11.2024 - Первая версия документа
- Найдено решение с использованием RecordUndoEvent
- Документированы основные аспекты реализации

## Рекомендации по использованию
1. Всегда инициализируйте lastPosition в конструкторе
2. Проверяйте наличие связанных объектов перед обновлением
3. Используйте отладочные сообщения для мониторинга перемещений
4. Обеспечьте корректное обновление документа после перемещений
```

Этот документ содержит всю необходимую информацию для реализации синхронного перемещения связанных объектов в будущих проектах.