using System;
using Animancer;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Data.Character.WeaponsConfigs
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/Weapon/ShootingWeaponConfig")]
    public sealed class ShootingWeaponConfig : ScriptableObject
    {
        [Title("Weapon General Settings")] // Основные настройки оружия
        [VerticalGroup("General", PaddingTop = 10)] // Логическая группировка
        [PreviewField(100, ObjectFieldAlignment.Left)] // Отображение иконки с увеличением
        [HideLabel] // Скрываем стандартную подпись для PreviewField
        public Sprite WeaponIcon; // Иконка оружия

        [VerticalGroup("General")] // Оружие типа
        [LabelText("Weapon Rich Text Ammo")]
        public string RichTextAmmo; // Тип оружия

        [TabGroup("Ammo & Performance", "Ammo")] // Вкладка "Боеприпасы"
        [LabelText("Total Ammo")]
        [Tooltip("Общее количество патронов")]
        public int TotalAmmo = 120; // Общее количество патронов

        [TabGroup("Ammo & Performance", "Ammo")]
        [LabelText("Magazine Capacity")]
        [Tooltip("Максимальное количество патронов в обойме")]
        public int MagazineCapacity = 30; // Патроны в обойме

        [TabGroup("Ammo & Performance", "Performance")]
        [LabelText("Damage")]
        [Tooltip("Урон")]
        public int Damage = 30; // Урон

        [TabGroup("Ammo & Performance", "Performance")] // Вкладка "Производительность"
        [LabelText("Range")]
        [Tooltip("Дальность стрельбы")]
        public float Range = 100f; // Дальность стрельбы

        [TabGroup("Ammo & Performance", "Performance")]
        [LabelText("Fire Rate")]
        [Tooltip("Скорострельность (выстрелов в секунду)")]
        public float FireRate = 10f; // Скорострельность

        [TabGroup("Ammo & Performance", "Performance")]
        [LabelText("Impact Force")]
        [Tooltip("Сила воздействия при попадании")]
        public float ImpactForce = 50f; // Сила при попадании

        [TabGroup("Visual & Effects", "Effects")] // Вкладка "Эффекты"
        [LabelText("Muzzle Flash Effect")]
        [Tooltip("Эффект вспышки выстрела")]
        [AssetsOnly]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)] // Встроенное превью
        public ParticleSystem MuzzleFlashEffect; // Эффект вспышки

        [TabGroup("Visual & Effects", "Effects")]
        [LabelText("Shell Ejection Effect")]
        [Tooltip("Эффект вылета гильз")]
        [AssetsOnly]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public ParticleSystem ShellEjectionEffect; // Эффект гильз

        [TabGroup("Visual & Effects", "Effects")]
        [LabelText("Hit Effect (Enemy)")]
        [Tooltip("Эффект при попадании во врага")]
        [AssetsOnly]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public ParticleSystem BloodEffect; // Эффект при попадании во врага

        [TabGroup("Visual & Effects", "Effects")]
        [LabelText("Hit Effect (Surface)")]
        [Tooltip("Эффект при попадании по поверхности")]
        [AssetsOnly]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public ParticleSystem SurfaceImpactEffect; // Эффект попадания

        [TabGroup("Visual & Effects", "Effects")]
        [LabelText("Bullet Hole Decal")]
        [Tooltip("Декаль дыры от пули")]
        [AssetsOnly]
        [InlineEditor(InlineEditorObjectFieldModes.Boxed)]
        public ParticleSystem BulletHoleDecal; // Декаль от пули
    }
}
