using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using GachaLife.Models;

namespace GachaLife.Animations;

// Все анимации выпадения наград живут здесь.
// Хотите изменить скорость или силу эффекта? Меняйте числа в блоке «Настройки».
public static class PullAnimations
{
    // ===== Настройки: общие =====

    // Задержка между карточками в серии (при ×10 они появляются волной)
    private const int StaggerMs = 90;

    // Длительность «прыжка» карточки и появления из прозрачности
    private const int PopDurationMs = 380;
    private const int FadeDurationMs = 250;

    // ===== Настройки: прыжок =====
    private const double Star3StartScale = 0.6;
    private const double Star3Amplitude = 0.6;
    private const double Star4StartScale = 0.5;
    private const double Star4Amplitude = 0.9;
    private const double Star5StartScale = 0.3;
    private const double Star5Amplitude = 1.3;

    // ===== Настройки: свечение =====
    // Flash: вспышка при появлении (размытие и яркость).
    // Min/Max: между этими значениями свечение постоянно «дышит».
    // PulseMs: время одного «вдоха» (от Min до Max), чем меньше, тем быстрее дыхание.
    private sealed record GlowStyle(
        double FlashBlur, double FlashOpacity,
        double MinBlur, double MaxBlur,
        double MinOpacity, double MaxOpacity,
        int PulseMs);

    private static readonly GlowStyle Star4Glow = new(
        FlashBlur: 28, FlashOpacity: 0.9,
        MinBlur: 12, MaxBlur: 26,
        MinOpacity: 0.35, MaxOpacity: 0.7,
        PulseMs: 1600);

    private static readonly GlowStyle Star5Glow = new(
        FlashBlur: 46, FlashOpacity: 1.0,
        MinBlur: 20, MaxBlur: 42,
        MinOpacity: 0.5, MaxOpacity: 0.95,
        PulseMs: 1400);

    // ===== Анимации =====

    // Карточка результата появилась на экране.
    // isFirstTime = true: игрок только что выбил награду (прыжок и вспышка).
    // isFirstTime = false: карточка просто вернулась на экран (только «дыхание» свечения).
    public static void PlayResultCard(Border card, int index, Rarity rarity, string colorHex, bool isFirstTime)
    {
        var delay = isFirstTime ? Ms(index * StaggerMs) : TimeSpan.Zero;
        var color = (Color)ColorConverter.ConvertFromString(colorHex);

        switch (rarity)
        {
            case Rarity.Star5:
                if (isFirstTime) Pop(card, delay, Star5StartScale, Star5Amplitude, wobble: true);
                Glow(card, color, delay, Star5Glow, withFlash: isFirstTime);
                break;

            case Rarity.Star4:
                if (isFirstTime) Pop(card, delay, Star4StartScale, Star4Amplitude, wobble: false);
                Glow(card, color, delay, Star4Glow, withFlash: isFirstTime);
                break;

            default:
                if (isFirstTime) Pop(card, delay, Star3StartScale, Star3Amplitude, wobble: false);
                break;
        }
    }

    // «Прыжок» из маленького размера с плавным появлением (и покачиванием, если wobble)
    private static void Pop(Border card, TimeSpan delay, double startScale, double amplitude, bool wobble)
    {
        var scale = new ScaleTransform(startScale, startScale);
        var rotate = new RotateTransform(0);
        var group = new TransformGroup();
        group.Children.Add(scale);
        group.Children.Add(rotate);

        card.RenderTransformOrigin = new Point(0.5, 0.5);
        card.RenderTransform = group;
        card.Opacity = 0;

        var grow = new DoubleAnimation(startScale, 1, Ms(PopDurationMs))
        {
            BeginTime = delay,
            EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = amplitude }
        };
        scale.BeginAnimation(ScaleTransform.ScaleXProperty, grow);
        scale.BeginAnimation(ScaleTransform.ScaleYProperty, grow);

        var fade = new DoubleAnimation(0, 1, Ms(FadeDurationMs)) { BeginTime = delay };
        card.BeginAnimation(UIElement.OpacityProperty, fade);

        if (wobble)
        {
            // Лёгкое покачивание в конце прыжка: влево, вправо, и на место
            var sway = new DoubleAnimationUsingKeyFrames { BeginTime = delay + Ms(PopDurationMs * 0.6) };
            sway.KeyFrames.Add(new LinearDoubleKeyFrame(-4, KeyTime.FromTimeSpan(Ms(80))));
            sway.KeyFrames.Add(new LinearDoubleKeyFrame(4, KeyTime.FromTimeSpan(Ms(200))));
            sway.KeyFrames.Add(new LinearDoubleKeyFrame(-2, KeyTime.FromTimeSpan(Ms(300))));
            sway.KeyFrames.Add(new LinearDoubleKeyFrame(0, KeyTime.FromTimeSpan(Ms(380))));
            rotate.BeginAnimation(RotateTransform.AngleProperty, sway);
        }
    }

    // Свечение: вспышка при появлении (если withFlash), затем бесконечное «дыхание»
    private static void Glow(Border card, Color color, TimeSpan delay, GlowStyle style, bool withFlash)
    {
        var glow = new DropShadowEffect
        {
            Color = color,
            ShadowDepth = 0,
            BlurRadius = withFlash ? 0 : style.MinBlur,
            Opacity = withFlash ? 0 : style.MinOpacity
        };
        card.Effect = glow;

        // Пока карточка на экране, дыхание можно запускать; когда она ушла, всё останавливаем
        bool isAlive = true;
        card.Unloaded += (_, _) =>
        {
            isAlive = false;
            glow.BeginAnimation(DropShadowEffect.BlurRadiusProperty, null);
            glow.BeginAnimation(DropShadowEffect.OpacityProperty, null);
        };

        if (!withFlash)
        {
            StartPulse(glow, style);
            return;
        }

        // Вспышка: быстро до пика, потом спад до нижней границы «дыхания»
        var start = delay + Ms(150);

        var blur = new DoubleAnimationUsingKeyFrames { BeginTime = start };
        blur.KeyFrames.Add(new EasingDoubleKeyFrame(style.FlashBlur, KeyTime.FromTimeSpan(Ms(250))));
        blur.KeyFrames.Add(new EasingDoubleKeyFrame(style.MinBlur, KeyTime.FromTimeSpan(Ms(900))));

        var opacity = new DoubleAnimationUsingKeyFrames { BeginTime = start };
        opacity.KeyFrames.Add(new EasingDoubleKeyFrame(style.FlashOpacity, KeyTime.FromTimeSpan(Ms(250))));
        opacity.KeyFrames.Add(new EasingDoubleKeyFrame(style.MinOpacity, KeyTime.FromTimeSpan(Ms(900))));

        // Когда вспышка закончилась, запускаем бесконечное дыхание
        blur.Completed += (_, _) =>
        {
            if (isAlive) StartPulse(glow, style);
        };

        glow.BeginAnimation(DropShadowEffect.BlurRadiusProperty, blur);
        glow.BeginAnimation(DropShadowEffect.OpacityProperty, opacity);
    }

    // Бесконечное «дыхание»: от минимума к максимуму и обратно
    private static void StartPulse(DropShadowEffect glow, GlowStyle style)
    {
        var period = Ms(style.PulseMs);
        var ease = new SineEase { EasingMode = EasingMode.EaseInOut };

        glow.BeginAnimation(DropShadowEffect.BlurRadiusProperty,
            new DoubleAnimation(style.MinBlur, style.MaxBlur, period)
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = ease
            });

        glow.BeginAnimation(DropShadowEffect.OpacityProperty,
            new DoubleAnimation(style.MinOpacity, style.MaxOpacity, period)
            {
                AutoReverse = true,
                RepeatBehavior = RepeatBehavior.Forever,
                EasingFunction = ease
            });
    }

    private static TimeSpan Ms(double milliseconds) => TimeSpan.FromMilliseconds(milliseconds);
}