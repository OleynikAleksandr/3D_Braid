Ничего страшного, давайте попробуем на основе того что есть. Я вижу в коде GH_NumberSliderAttributes, что размер области для никнейма вычисляется на основе размера текста:

1. Сначала измеряется размер текста
2. Добавляется отступ 10 пикселей
3. Устанавливается минимальный размер равный высоте

Может стоит попробовать установить фиксированную ширину для области никнейма? Попробуем:

```csharp
private void CreateSlider(GH_Document document, IGH_Param param, string nickName, double defaultValue, double minValue, double maxValue, float sliderOffset)
{
    if (param is null) return;

    // Измеряем ширину никнейма заранее
    float nicknameWidth = GH_FontServer.StringWidth(nickName, GH_FontServer.StandardAdjusted) + 10f;
    // Устанавливаем минимальную ширину для всех никнеймов
    nicknameWidth = Math.Max(nicknameWidth, 35f);  // 35 пикселей как пример

    var slider = new GH_NumberSlider();
    slider.CreateAttributes();

    if (this.Attributes?.DocObject?.Attributes != null)
    {
        slider.Attributes.Pivot = new PointF(
            (float)this.Attributes.DocObject.Attributes.Bounds.Left - sliderOffset + (nicknameWidth - 20f),  // Сдвигаем с учетом ширины
            (float)param.Attributes.Bounds.Y
        );

        slider.Name = nickName;
        slider.NickName = nickName;
        slider.Description = param.Description;

        // ... остальной код создания слайдера ...
    }
}
```

Суть в том, чтобы:
1. Заранее рассчитать ширину области под никнейм
2. Скорректировать позицию слайдера с учетом этой ширины

ПОПРОБОВАЛИ - просто сдвигает в право весь слайдер, ближе к модулю