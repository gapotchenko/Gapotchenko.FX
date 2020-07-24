#if TF_NET_FRAMEWORK

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Gapotchenko.FX.Drawing
{
    /// <summary>
    /// Provides access to theme colors of a host system.
    /// The difference between <see cref="ThemeColors"/> and <see cref="SystemColors"/> is that theme colors are the real colors shown on screen,
    /// while <see cref="SystemColors"/> provides just a compatible approximation.
    /// </summary>
    [Obsolete("ThemeColors experimental class may change its API or may be moved to another package in future versions of Gapotchenko.FX.")]
    public static class ThemeColors
    {
        static ThemeColors()
        {
            _ThemeEngine = _CreateThemeEngine();

            SystemEvents.UserPreferenceChanging += SystemEvents_UserPreferenceChanging;
        }

        abstract class ThemeEngine
        {
            public abstract Color GetKnownColor(KnownColor color);

            public virtual void Refresh()
            {
            }
        }

        class ThemeEngineDefault : ThemeEngine
        {
            public override Color GetKnownColor(KnownColor color) => Color.FromKnownColor(color);
        }

        class ThemeEngineWindowsGeneric : ThemeEngineDefault
        {
            protected static Color COLORREFToColor(int value) =>
                Color.FromArgb(
                    (byte)value,
                    (byte)(value >> 8),
                    (byte)(value >> 16));
        }

        class ThemeEngineWindowsXP : ThemeEngineWindowsGeneric
        {
            Color? _WindowFrameColor;
            Color? _InfoColor;
            Color? _InfoTextColor;

            bool _Initialized;

            void _EnsureInitialized()
            {
                if (!_Initialized)
                {
                    _Initialize();
                    _Initialized = true;
                }
            }

            void _Initialize()
            {
                _WindowFrameColor = null;
                _InfoColor = null;
                _InfoTextColor = null;

                if (VisualStyleRenderer.IsSupported && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ToolTip.Standard.Normal))
                {
                    try
                    {
                        var renderer = new VisualStyleRenderer(VisualStyleElement.ToolTip.Standard.Normal);

                        var canvasSize = new Size(7, 7);

                        using (var bmp = new Bitmap(canvasSize.Width, canvasSize.Height))
                        {
                            using (var graphics = Graphics.FromImage(bmp))
                                renderer.DrawBackground(graphics, new Rectangle(Point.Empty, canvasSize));

                            _InfoColor = bmp.GetPixel(canvasSize.Width / 2, canvasSize.Height / 2);
                            _WindowFrameColor = bmp.GetPixel(0, canvasSize.Height / 2);
                        }

                        _InfoTextColor = renderer.GetColor(ColorProperty.TextColor);
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                }
            }

            public override Color GetKnownColor(KnownColor color)
            {
                switch (color)
                {
                    case KnownColor.WindowFrame:
                        _EnsureInitialized();
                        if (_WindowFrameColor.HasValue)
                            return _WindowFrameColor.Value;
                        break;
                    case KnownColor.Info:
                        _EnsureInitialized();
                        if (_InfoColor.HasValue)
                            return _InfoColor.Value;
                        break;
                    case KnownColor.InfoText:
                        _EnsureInitialized();
                        if (_InfoTextColor.HasValue)
                            return _InfoTextColor.Value;
                        break;
                }

                return Color.FromKnownColor(color);
            }

            public override void Refresh()
            {
                _Initialized = false;
                base.Refresh();
            }
        }

        class ThemeEngineWindowsVista : ThemeEngineWindowsXP
        {
            protected static bool IsAeroGlassEnabled => NativeMethods.DwmIsCompositionEnabled(out var enabled) == 0 && enabled;
        }

        class ThemeEngineWindows8 : ThemeEngineWindowsVista
        {
            // https://stackoverflow.com/questions/31091298/how-to-get-window-title-bar-active-and-inactive-color-in-windows-8
        }

        class ThemeEngineWindows10 : ThemeEngineWindowsVista
        {
            static bool _ShouldCustomDrawSystemTitlebar => !SystemInformation.HighContrast;

            static bool _ShouldUseNativeFrame => _ShouldCustomDrawSystemTitlebar && IsAeroGlassEnabled;

            static bool _DwmColorsAllowed => _ShouldUseNativeFrame;

            bool _Initialized;

            void _EnsureInitialized()
            {
                if (!_Initialized)
                {
                    _Initialize();
                    _Initialized = true;
                }
            }

            // Returns a blend of the supplied colors, ranging from |background| (for
            // |alpha| == 0) to |foreground| (for |alpha| == 255). The alpha channels of
            // the supplied colors are also taken into account, so the returned color may
            // be partially transparent.
            static Color _AlphaBlend(Color foreground, Color background, int alpha)
            {
                if (alpha == 0)
                    return background;
                if (alpha == 255)
                    return foreground;

                int f_alpha = foreground.A;
                int b_alpha = background.A;

                double normalizer = (f_alpha * alpha + b_alpha * (255 - alpha)) / 255.0;
                if (normalizer == 0.0)
                    return Color.Transparent;

                double f_weight = f_alpha * alpha / normalizer;
                double b_weight = b_alpha * (255 - alpha) / normalizer;

                double r = (foreground.R * f_weight + background.R * b_weight) / 255.0;
                double g = (foreground.G * f_weight + background.G * b_weight) / 255.0;
                double b = (foreground.B * f_weight + background.B * b_weight) / 255.0;

                return Color.FromArgb(
                    (int)Math.Round(normalizer),
                    (int)Math.Round(r),
                    (int)Math.Round(g),
                    (int)Math.Round(b));
            }

            void _Initialize()
            {
                // https://chromium.googlesource.com/chromium/src.git/+/62.0.3178.1/chrome/browser/themes/theme_service_win.cc

                _DwmFrameColor = null;
                _DwmInactiveFrameColor = null;
                _DwmInactiveFrameColorGuess = null;
                _DwmAccentBorderColor = Color.White;

                using (var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM"))
                {
                    if (key == null)
                        return;

                    bool useDwmFrameColor =
                        key.TryGetInt32Value("AccentColor", out var accentColor) &&
                        key.TryGetInt32Value("ColorPrevalence", out var colorPrevalence) &&
                        colorPrevalence == 1;

                    if (useDwmFrameColor)
                    {
                        _DwmFrameColor = COLORREFToColor(accentColor);
                        if (key.TryGetInt32Value("AccentColorInactive", out var accentColorInactive))
                        {
                            _DwmInactiveFrameColor = COLORREFToColor(accentColorInactive);
                        }
                        else
                        {
                            // TODO: Is there any other way to get the color?
                            _DwmInactiveFrameColorGuess = Color.White;
                        }
                    }

                    if (key.TryGetInt32Value("ColorizationColor", out var colorizationColor) &&
                        key.TryGetInt32Value("ColorizationColorBalance", out var colorizationColorBalance))
                    {
                        // The accent border color is a linear blend between the colorization
                        // color and the neutral #d9d9d9. colorization_color_balance is the
                        // percentage of the colorization color in that blend.
                        //
                        // On Windows version 1611 colorization_color_balance can be 0xfffffff3 if
                        // the accent color is taken from the background and either the background
                        // is a solid color or was just changed to a slideshow. It's unclear what
                        // that value's supposed to mean, so change it to 80 to match Edge's
                        // behavior.
                        if (colorizationColorBalance < 0 || colorizationColorBalance > 100)
                            colorizationColorBalance = 80;

                        // Colorization color's high byte is not an alpha value, so replace it
                        // with 0xff to make an opaque ARGB color.
                        var inputColor = Color.FromArgb(colorizationColor & 0x00ffffff | unchecked((int)0xff000000));

                        _DwmAccentBorderColor = _AlphaBlend(
                            inputColor,
                            Color.FromArgb(0xd9, 0xd9, 0xd9),
                            (int)Math.Round(255.0 * colorizationColorBalance / 100.0));
                    }
                }
            }

            public override void Refresh()
            {
                _Initialized = false;
                base.Refresh();
            }

            Color? _DwmFrameColor;
            Color? _DwmInactiveFrameColor, _DwmInactiveFrameColorGuess;
            Color _DwmAccentBorderColor;

            public override Color GetKnownColor(KnownColor color)
            {
                if (_DwmColorsAllowed)
                {
                    switch (color)
                    {
                        case KnownColor.ActiveCaption:
                            _EnsureInitialized();
                            if (_DwmFrameColor.HasValue)
                                return _DwmFrameColor.Value;
                            if (!_ShouldCustomDrawSystemTitlebar)
                                return Color.White;
                            break;

                        case KnownColor.InactiveCaption:
                            _EnsureInitialized();
                            if (_DwmInactiveFrameColor.HasValue)
                                return _DwmInactiveFrameColor.Value;
                            if (!_ShouldCustomDrawSystemTitlebar)
                                return Color.White;
                            if (_DwmInactiveFrameColorGuess.HasValue)
                                return _DwmInactiveFrameColorGuess.Value;
                            break;

                        case KnownColor.ActiveBorder:
                            _EnsureInitialized();
                            return _DwmAccentBorderColor;

                        case KnownColor.InactiveBorder:
                            // The accent border has to be opaque, but native inactive borders are #565656 with 80% alpha.
                            // We copy Edge (which also custom-draws its top border) and use #A2A2A2 instead.
                            return Color.FromArgb(unchecked((int)0xffa2a2a2));
                    }
                }

                return base.GetKnownColor(color);
            }
        }

        static readonly ThemeEngine _ThemeEngine;

        static ThemeEngine _CreateThemeEngine()
        {
            var os = Environment.OSVersion;
            var osVersion = os.Version;

            if (os.Platform == PlatformID.Win32NT)
            {
                if (osVersion >= new Version(10, 0))
                    return new ThemeEngineWindows10();
                else if (osVersion >= new Version(6, 2))
                    return new ThemeEngineWindows8();
                else if (osVersion >= new Version(6, 0))
                    return new ThemeEngineWindowsVista();
                else
                    return new ThemeEngineWindowsGeneric();
            }
            else
            {
                return new ThemeEngineDefault();
            }
        }

        static bool _IsSystemColor(KnownColor color)
        {
            switch (color)
            {
                case KnownColor.ActiveBorder:
                case KnownColor.ActiveCaption:
                case KnownColor.ActiveCaptionText:
                case KnownColor.AppWorkspace:
                case KnownColor.ButtonFace:
                case KnownColor.ButtonHighlight:
                case KnownColor.ButtonShadow:
                case KnownColor.Control:
                case KnownColor.ControlDark:
                case KnownColor.ControlDarkDark:
                case KnownColor.ControlLight:
                case KnownColor.ControlLightLight:
                case KnownColor.ControlText:
                case KnownColor.Desktop:
                case KnownColor.GradientActiveCaption:
                case KnownColor.GradientInactiveCaption:
                case KnownColor.GrayText:
                case KnownColor.Highlight:
                case KnownColor.HighlightText:
                case KnownColor.HotTrack:
                case KnownColor.InactiveBorder:
                case KnownColor.InactiveCaption:
                case KnownColor.InactiveCaptionText:
                case KnownColor.Info:
                case KnownColor.InfoText:
                case KnownColor.Menu:
                case KnownColor.MenuBar:
                case KnownColor.MenuHighlight:
                case KnownColor.MenuText:
                case KnownColor.ScrollBar:
                case KnownColor.Window:
                case KnownColor.WindowFrame:
                case KnownColor.WindowText:
                    return true;

                default:
                    return false;
            }
        }

        static Dictionary<KnownColor, Color> _CachedColors = new Dictionary<KnownColor, Color>();

        /// <summary>
        /// Gets a value of the specified predefined color.
        /// </summary>
        /// <param name="color">An element of the <see cref="KnownColor"/> enumeration.</param>
        /// <returns>The color value.</returns>
        public static Color GetKnownColor(KnownColor color)
        {
            if (_IsSystemColor(color))
            {
                var cachedColors = _CachedColors;
                lock (cachedColors)
                {
                    Color value;
                    if (cachedColors.TryGetValue(color, out value))
                        return value;
                    value = _ThemeEngine.GetKnownColor(color);
                    cachedColors.Add(color, value);
                    return value;
                }
            }
            else
            {
                return Color.FromKnownColor(color);
            }
        }

        /// <summary>
        /// Discards any information about theme colors that has been cached.
        /// </summary>
        public static void Refresh()
        {
            var cachedColors = _CachedColors;
            lock (cachedColors)
            {
                cachedColors.Clear();
                _ThemeEngine.Refresh();
            }
        }

        static void SystemEvents_UserPreferenceChanging(object sender, UserPreferenceChangingEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.General || e.Category == UserPreferenceCategory.Color)
                Refresh();
        }

        /// <summary>
        /// Gets the color of the active window's border.
        /// </summary>
        public static Color ActiveBorder => GetKnownColor(KnownColor.ActiveBorder);

        /// <summary>
        /// Gets the color of the background of the active window's title bar.
        /// </summary>
        public static Color ActiveCaption => GetKnownColor(KnownColor.ActiveCaption);

        /// <summary>
        /// Gets the color of an inactive window's border.
        /// </summary>
        public static Color InactiveBorder => GetKnownColor(KnownColor.InactiveBorder);

        /// <summary>
        /// Gets the color of the background of the inactive window's title bar.
        /// </summary>
        public static Color InactiveCaption => GetKnownColor(KnownColor.InactiveCaption);

        /// <summary>
        /// Gets the color of the window's frame.
        /// </summary>
        public static Color WindowFrame => GetKnownColor(KnownColor.WindowFrame);

        /// <summary>
        /// Gets the color of the background of a ToolTip.
        /// </summary>
        public static Color Info => GetKnownColor(KnownColor.Info);

        /// <summary>
        /// Gets the color of the text of a ToolTip.
        /// </summary>
        public static Color InfoText => GetKnownColor(KnownColor.InfoText);
    }
}

#endif
