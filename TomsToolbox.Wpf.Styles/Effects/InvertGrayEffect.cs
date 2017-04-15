namespace TomsToolbox.Wpf.Styles.Effects
{
    using System.Windows;
    using System.Windows.Media.Effects;

    using JetBrains.Annotations;

    using TomsToolbox.Core;

    /// <summary>
    /// Shader effect that inverts all gray values but leaves colors untouched.
    /// </summary>
    /// <seealso cref="System.Windows.Media.Effects.ShaderEffect" />
    public class InvertGrayEffect : ShaderEffect
    {
        [NotNull] private static readonly PixelShader _pixelShader = new PixelShader() { UriSource = typeof(InvertGrayEffect).Assembly.GeneratePackUri("effects/invertGray.ps") };
        [NotNull] private static readonly DependencyProperty _inputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(InvertGrayEffect), 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="InvertGrayEffect"/> class.
        /// </summary>
        public InvertGrayEffect()
        {
            PixelShader = _pixelShader;
            UpdateShaderValue(_inputProperty);
        }
    }
}
